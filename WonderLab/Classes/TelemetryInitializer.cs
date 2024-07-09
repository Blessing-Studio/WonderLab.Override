using WonderLab.Services.Download;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using System;

namespace WonderLab.Classes;
public sealed partial class TelemetryInitializer : ITelemetryInitializer {
    public void Initialize(ITelemetry telemetry) {
        var token = Guid.NewGuid().ToString();
        telemetry.Context.User.Id = Environment.UserName;
        telemetry.Context.Session.Id = token;
        telemetry.Context.Cloud.RoleName = "WonderLab.Override";
        telemetry.Context.Cloud.RoleInstance = "WonderLab.Override";
        telemetry.Context.Component.Version = UpdateService.Version;
        //telemetry.Context.GlobalProperties["Branch"] = BranchSymbol.CurrentDes;
        //telemetry.Context.GlobalProperties["BranchFramework"] = BranchSymbol.Framework;
        //telemetry.Context.GlobalProperties["BranchRuntime"] = BranchSymbol.Runtime;
    }
}