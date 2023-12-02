using System;
using Flurl.Http;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Nodes;
using WonderLab.Classes.Enums;
using WonderLab.Classes.Managers;
using System.Collections.Generic;
using WonderLab.Classes.Extension;
using System.Diagnostics;
using System.IO;

namespace WonderLab.Classes.Handlers {
    public class UpdateHandler {
        private DataManager _dataManager;

        private readonly string _baseUrl = "http://s2.fxidc.net:2999/api/update/";

        public JsonNode UpdateInfoJsonNode { get; set; }

        public UpdateHandler(DataManager dataManager) { 
            _dataManager = dataManager;
        }

        private async Task InitAsync() {
            var branch = _dataManager.Config.Branch switch {
                BranchType.Lsaac => "lsaac",
                BranchType.Albert => "albert",
                _ => "lsaac"
            };

            var url = $"{_baseUrl}{branch}";
            if(string.IsNullOrEmpty(url)) {
                return;
            }

            try {
                UpdateInfoJsonNode = await JsonNode.ParseAsync(await url
                    .GetStreamAsync()) ?? default!;
            }
            catch (Exception) {
                return;
            }
        }

        public async ValueTask<bool> CheckAsync() {
            await InitAsync();
            if (UpdateInfoJsonNode is null) {
                return false;
            }

            int remoteVersion = UpdateInfoJsonNode["version"]!
                .GetValue<string>()
                .Split('.')
                .ElementAtOrDefault(1)!
                .ToInt();

            int localVersion = _dataManager.Version
                .Split(".")
                .ElementAtOrDefault(1)!
                .ToInt();

            return remoteVersion > localVersion;
        }

        public void Update() {
            int currentPID = Process.GetCurrentProcess().Id;
            string name = Process.GetCurrentProcess().ProcessName,
                filename = $"{name}.exe";

            var psCommand = $"Stop-Process -Id {currentPID} -Force;" +
                $"Wait-Process -Id {currentPID} -ErrorAction SilentlyContinue;" +
                $"Start-Sleep -Milliseconds 500;" +
                $"Remove-Item updateTemp.zip -Force;" +
                $"Remove-Item {filename} -Force;" +
                $"Rename-Item launcher.temp {filename};" +
                $"Start-Process {name}.exe -Args updated;";
            try {
                using var process = Process.Start(new ProcessStartInfo {
                    FileName = "powershell.exe",
                    Arguments = psCommand,
                    WorkingDirectory = Directory.GetCurrentDirectory(),
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                });
            }
            catch (Exception) {
            }
        }
    }
}
