using System;
using System.Reflection;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;

namespace WonderLab.Services.Download;

public sealed class UpdateService {
    private const string BASE_API = "http://47.113.149.130:14514/api/update";

    private readonly ILogger<UpdateService> _logger;
    private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();

    public static string Version =>
        (Attribute.GetCustomAttribute(_assembly, typeof(AssemblyFileVersionAttribute), false)
        as AssemblyFileVersionAttribute).Version;


    public UpdateService(ILogger<UpdateService> logger) {
        _logger = logger;
        _logger.LogInformation("初始化更新服务");
    }

    public async ValueTask CheckAsync() {
        _logger.LogInformation("开始检查启动器本体更新");
        var branch = Version.Contains("pre") ? "lsaac" : "albert";
        var checkUrl = Url.Combine(BASE_API, branch, Version);
        var result = await checkUrl.GetStringAsync();
    }
}
