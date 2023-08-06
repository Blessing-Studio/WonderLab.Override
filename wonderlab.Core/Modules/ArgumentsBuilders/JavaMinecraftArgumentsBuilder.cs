using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MinecraftLaunch.Modules.Interface;
using MinecraftLaunch.Modules.Models.Download;
using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Toolkits;
using Natsurainko.Toolkits.Values;

namespace MinecraftLaunch.Modules.ArgumentsBuilders;

public sealed partial class JavaMinecraftArgumentsBuilder : IArgumentsBuilder {
    public static readonly IEnumerable<string> DefaultAdvancedArguments = new string[8] { "-XX:-OmitStackTraceInFastThrow", "-XX:-DontCompileHugeMethods", "-Dfile.encoding=GB18030", "-Dfml.ignoreInvalidMinecraftCertificates=true", "-Dfml.ignorePatchDiscrepancies=true", "-Djava.rmi.server.useCodebaseOnly=true", "-Dcom.sun.jndi.rmi.object.trustURLCodebase=false", "-Dcom.sun.jndi.cosnaming.object.trustURLCodebase=false" };

    public static readonly IEnumerable<string> DefaultGCArguments = new string[7] { "-XX:+UseG1GC", "-XX:+UnlockExperimentalVMOptions", "-XX:G1NewSizePercent=20", "-XX:G1ReservePercent=20", "-XX:MaxGCPauseMillis=50", "-XX:G1HeapRegionSize=16m", "-XX:-UseAdaptiveSizePolicy" };

    public bool EnableIndependencyCore { get; init; }

    public JavaMinecraftArgumentsBuilder(GameCore? gameCore, LaunchConfig? launchConfig, bool enableIndependencyCore = true) {
        GameCore = gameCore;
        LaunchConfig = launchConfig;
        EnableIndependencyCore = enableIndependencyCore;
    }

    public GameCore GameCore { get; private set; }

    public LaunchConfig LaunchConfig { get; private set; }

    public IEnumerable<string> Build() {
        foreach (string frontArgument in GetFrontArguments())
            yield return frontArgument;

        yield return GameCore.MainClass!;

        foreach (string behindArgument in GetBehindArguments())
            yield return behindArgument;
    }

    public IEnumerable<string> GetBehindArguments() {
        var keyValuePairs = new Dictionary<string, string>()
        {
            { "${auth_player_name}" , this.LaunchConfig.Account.Name },
            { "${version_name}" , this.GameCore.Id },
            { "${assets_root}" , Path.Combine(this.GameCore.Root.FullName, "assets").ToPath() },
            { "${assets_index_name}" , Path.GetFileNameWithoutExtension(this.GameCore.AssetIndexFile.FileInfo.FullName) },
            { "${auth_uuid}" , this.LaunchConfig.Account.Uuid.ToString("N") },
            { "${auth_access_token}" , this.LaunchConfig.Account.AccessToken },
            { "${user_type}" , "Mojang" },
            { "${version_type}" , "WonderLab.Override" },
            { "${user_properties}" , "{}" },
            { "${game_assets}" , Path.Combine(this.GameCore.Root.FullName, "assets").ToPath() },
            { "${auth_session}" , this.LaunchConfig.Account.AccessToken },
            {
                "${game_directory}" ,
                    (this.EnableIndependencyCore && (bool)this.LaunchConfig.WorkingFolder?.Exists
                        ? this.LaunchConfig.WorkingFolder.FullName
                        : GameCore.Root.FullName).ToPath()
            },
        };

        var args = this.GameCore.BehindArguments.ToList();

        if (this.LaunchConfig.GameWindowConfig != null) {
            args.Add($"--width {this.LaunchConfig.GameWindowConfig.Width}");
            args.Add($"--height {this.LaunchConfig.GameWindowConfig.Height}");

            if (this.LaunchConfig.GameWindowConfig.IsFullscreen)
                args.Add("--fullscreen");
        }

        //if (this.LaunchConfig.ServerSetting != null && !string.IsNullOrEmpty(this.LaunchConfig.ServerSetting.IPAddress) && this.LaunchConfig.ServerSetting.Port != 0) {
        //    args.Add($"--server {this.LaunchConfig.ServerSetting.IPAddress}");
        //    args.Add($"--port {this.LaunchConfig.ServerSetting.Port}");
        //}

        foreach (var item in args) {
            yield return item.Replace(keyValuePairs);
        }
    }

    public IEnumerable<string> GetFrontArguments() {
        var keyValuePairs = new Dictionary<string, string>() {       
            { "${launcher_name}", "MinecraftLaunch" },
            { "${launcher_version}", "3" },
            { "${classpath_separator}", Path.PathSeparator.ToString() },
            { "${classpath}", this.GetClasspath().ToPath() },
            { "${client}", this.GameCore.ClientFile.FileInfo.FullName.ToPath() },
            { "${min_memory}", this.LaunchConfig.JvmConfig.MinMemory.ToString() },
            { "${max_memory}", this.LaunchConfig.JvmConfig.MaxMemory.ToString() },
            { "${library_directory}", Path.Combine(this.GameCore.Root.FullName, "libraries").ToPath() },
            {
                "${version_name}",
                string.IsNullOrEmpty(this.GameCore.InheritsFrom!)
                ? this.GameCore.Id
                : this.GameCore.InheritsFrom!
            },
            {
                "${natives_directory}",
                this.LaunchConfig.NativesFolder != null && this.LaunchConfig.NativesFolder.Exists
                ? this.LaunchConfig.NativesFolder.FullName.ToString()
                : Path.Combine(this.GameCore.Root.FullName, "versions", this.GameCore.Id, "natives").ToPath()
            }
        };

        if (!Directory.Exists(keyValuePairs["${natives_directory}"])) {
            Directory.CreateDirectory(keyValuePairs["${natives_directory}"].Trim('\"'));
        }

        var args = new string[] {       
            "-Xmn${min_memory}m",
            "-Xmx${max_memory}m",
            "-Dminecraft.client.jar=${client}",
        }.ToList();

        foreach (var item in GetEnvironmentJvmArguments()) {
            args.Add(item);
        }

        if (this.LaunchConfig.JvmConfig.GCArguments == null) {
            DefaultGCArguments.ToList().ForEach(args.Add);
        } else {
            this.LaunchConfig.JvmConfig.GCArguments.ToList().ForEach(args.Add);
        }

        if (this.LaunchConfig.JvmConfig.AdvancedArguments == null) {
            DefaultAdvancedArguments.ToList().ForEach(args.Add);

        } else {
            this.LaunchConfig.JvmConfig.AdvancedArguments.ToList().ForEach(args.Add);
        }

        args.Add("-Dlog4j2.formatMsgNoLookups=true");

        foreach (var item in this.GameCore.FrontArguments) {
            args.Add(item);
        }

        foreach (var item in args) {
            yield return item.Replace(keyValuePairs);
        }
    }

    private string GetClasspath() {
        var loads = new List<IResource>();

        this.GameCore.LibraryResources!.ForEach(x => {
            if (x.IsEnable && !x.IsNatives)
                loads.Add(x);
        });

        loads.Add(this.GameCore.ClientFile!);

        return string.Join(Path.PathSeparator.ToString(), loads.Select(x => x.ToFileInfo().FullName));
    }

    private static IEnumerable<string> GetEnvironmentJvmArguments() {
        switch (EnvironmentToolkit.GetPlatformName()) {
            case "windows":
                yield return "-XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump";

                if (Environment.OSVersion.Version.Major == 10) {
                    yield return "-Dos.name=\"Windows 10\"";
                    yield return "-Dos.version=10.0";
                }
                break;
            case "osx":
                yield return "-XstartOnFirstThread";
                break;
        }

        if (EnvironmentInfo.Arch == "32")
            yield return "-Xss1M";
    }
}
