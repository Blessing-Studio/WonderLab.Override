using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Buffers;
using System.Net.Http;
using System.Threading;
using Avalonia.Threading;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using WonderLab.Classes.Enums;
using WonderLab.Classes.Datas;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks.Dataflow;

namespace WonderLab.Services.Download;

/// <summary>
/// 下载服务类
/// </summary>
public sealed class DownloadService {
    private const int MAX_RETRY_COUNT = 3;
    private const int BUFFER_SIZE = 4096;
    private const double UPDATE_INTERVAL = 0.5;

    private CancellationTokenSource _userCts;
    private ImmutableList<DownloadItemData> _downloadItems;

    private int _totalBytes;
    private int _downloadedBytes;
    private int _previousDownloadedBytes;

    private int _totalCount;
    private int _completedCount;
    private int _failedCount;

    private readonly HttpClient _client;
    private readonly DispatcherTimer _timer;
    private readonly ArrayPool<byte> _bufferPool;
    private readonly SettingService _settingService;
    private readonly AutoResetEvent _autoResetEvent;
    private readonly ILogger<DownloadService> _logger;

    public event EventHandler<DownloadResult> Completed;
    public event EventHandler<DownloadProgressData> ProgressChanged;

    public DownloadService(SettingService settingService, ILogger<DownloadService> logger) {
        _logger = logger;
        _settingService = settingService;

        _client = new HttpClient();
        _client.DefaultRequestHeaders.Connection.Add("keep-alive");

        _bufferPool = ArrayPool<byte>.Create(BUFFER_SIZE, Environment.ProcessorCount * 2);
        _autoResetEvent = new AutoResetEvent(true);

        _userCts = new CancellationTokenSource();

        _timer = new DispatcherTimer() {
            Interval = TimeSpan.FromSeconds(UPDATE_INTERVAL)
        };

        _timer.Tick += (sender, e) => UpdateDownloadProgress();
    }

    public void Retry() {
        _logger.LogInformation("Retrying incomplete downloads");

        _failedCount = 0;
        _downloadItems = _downloadItems.Where(item => !item.IsCompleted).ToImmutableList();

        _autoResetEvent.Set();
    }

    public void Cancel() {
        _logger.LogInformation("Canceling downloads");

        _timer.Stop();
        _userCts.Cancel();
        _autoResetEvent.Set();
    }

    public void Setup(IEnumerable<DownloadItemData> downloadItems) {
        _downloadedBytes = 0;
        _previousDownloadedBytes = 0;
        _downloadItems = downloadItems.ToImmutableList();
        _totalBytes = _downloadItems.Sum(item => item.Size);

        _failedCount = 0;
        _completedCount = 0;
        _totalCount = _downloadItems.Count;

        if (_userCts.IsCancellationRequested) {
            _userCts.Dispose();
            _userCts = new CancellationTokenSource();
        }

        _autoResetEvent.Reset();
        _logger.LogInformation("New downloads added. Count: {totalCount} Size: {totalBytes} bytes", _totalCount, _totalCount);
    }

    public async ValueTask<bool> DownloadAllAsync() {
        while (true) {
            _timer.Start();

            try {
                var downloader = new ActionBlock<DownloadItemData>(async item => {
                    for (int i = 0; i < MAX_RETRY_COUNT && !_userCts.IsCancellationRequested; i++) {
                        if (await DownloadItemAsync(item, i, 8)) {
                            break;
                        }
                    }
                }, new ExecutionDataflowBlockOptions {
                    CancellationToken = _userCts.Token,
                    MaxDegreeOfParallelism = _settingService.Data.MultiThreadsCount
                });

                foreach (var item in _downloadItems) {
                    downloader.Post(item);
                }

                downloader.Complete();
                await downloader.Completion;
            } catch (OperationCanceledException) { }

            _timer.Stop();
            UpdateDownloadProgress();

            if (_completedCount == _totalCount) {
                Completed?.Invoke(this, DownloadResult.Succeeded);
                return true;
            }

            foreach (var item in _downloadItems) {
                if (!item.IsCompleted && File.Exists(item.Path)) {
                    File.Delete(item.Path);
                }
            }

            if (_failedCount > 0 && !_userCts.IsCancellationRequested) {
                Completed?.Invoke(this, DownloadResult.Incomplete);
            }

            _autoResetEvent.WaitOne();

            if (_userCts.IsCancellationRequested) {
                Completed?.Invoke(this, DownloadResult.Canceled);
                return false;
            }
        }
    }

    private void UpdateDownloadProgress() {
        int diffBytes = _downloadedBytes - _previousDownloadedBytes;
        _previousDownloadedBytes = _downloadedBytes;

        var progress = new DownloadProgressData {
            TotalCount = _totalCount,
            CompletedCount = _completedCount,
            FailedCount = _failedCount,
            TotalBytes = _totalBytes,
            DownloadedBytes = _downloadedBytes,
            Speed = diffBytes / UPDATE_INTERVAL,
        };

        ProgressChanged?.Invoke(this, progress);
    }

    private async ValueTask<bool> DownloadItemAsync(DownloadItemData item, int retryTimes, int chunkCount, long sizeThreshold = 1048576) {
        if (_userCts.IsCancellationRequested) {
            return true;
        }

        if (retryTimes > 0) {
            _logger.LogWarning("{Url}: Retrying {RetryTimes} times", item.Url, retryTimes);
        }

        // Make sure directory exists
        if (Path.IsPathRooted(item.Path)) {
            Directory.CreateDirectory(Path.GetDirectoryName(item.Path));
        }

        if (!File.Exists(item.Path)) {
            using (File.Create(item.Path)) { }
        }

        byte[] buffer = _bufferPool.Rent(BUFFER_SIZE);

        try {
            var request = new HttpRequestMessage(HttpMethod.Get, item.Url);
            var response =
                await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, _userCts.Token);

            if (response.StatusCode == HttpStatusCode.Found) {
                // Handle redirection
                request = new HttpRequestMessage(HttpMethod.Get, response.Headers.Location);
                response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead,
                    _userCts.Token);
            }

            if (item.Size == 0) {
                item.Size = (int)(response.Content.Headers.ContentLength ?? 0);
                Interlocked.Add(ref _totalBytes, item.Size);
            }

            item.IsPartialContentSupported = response.Headers.AcceptRanges.Contains("bytes");

            // Calculate the size of each chunk
            int chunkSize = (int)Math.Ceiling((double)item.Size / chunkCount);

            // Decide whether to use chunked download based on the size threshold
            bool useChunkedDownload = item.Size > sizeThreshold && item.IsPartialContentSupported;

            for (int i = 0; i < (useChunkedDownload ? chunkCount : 1); i++) {
                int chunkStart = i * chunkSize;
                int chunkEnd = useChunkedDownload ? Math.Min(chunkStart + chunkSize, item.Size) - 1 : item.Size - 1;

                request = new HttpRequestMessage(HttpMethod.Get, item.Url);
                request.Headers.Range = new RangeHeaderValue(chunkStart, chunkEnd);

                response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, _userCts.Token);

                await using var httpStream = await response.Content.ReadAsStreamAsync();
                await using var fileStream = new FileStream(item.Path, FileMode.Open, FileAccess.Write, FileShare.Write);
                fileStream.Position = chunkStart;

                int bytesRead;
                while ((bytesRead = await httpStream.ReadAsync(buffer, _userCts.Token)) > 0) {
                    await fileStream.WriteAsync(buffer, 0, bytesRead, _userCts.Token);
                    item.DownloadedBytes += bytesRead;
                    Interlocked.Add(ref _downloadedBytes, bytesRead);
                }
            }

            // Download successful
            item.IsCompleted = true;
            Interlocked.Increment(ref _completedCount);

            request.Dispose();
            response.Dispose();
            return true;
        } catch (OperationCanceledException) {
            if (!_userCts.IsCancellationRequested) {
                _logger.LogError("{Url}: Timeout", item.Url);
            }
        } catch (HttpRequestException ex) {
            _logger.LogError(ex, "{Url}: HTTP error occurred.", item.Url);
        } catch (Exception ex) {
            _logger.LogError(ex, "{Url}: Unkown error occurred.", item.Url);
        } finally {
            _bufferPool.Return(buffer);
        }

        if (!_userCts.IsCancellationRequested) {
            Interlocked.Increment(ref _failedCount);
            Interlocked.Add(ref _downloadedBytes, -item.DownloadedBytes);
            item.DownloadedBytes = 0;
            Interlocked.Exchange(ref _previousDownloadedBytes, _downloadedBytes);
        }

        return false;
    }
}