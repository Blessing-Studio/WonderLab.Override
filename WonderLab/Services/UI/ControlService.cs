using Avalonia.Controls;
using Avalonia.Input;

namespace WonderLab.Services.UI;
public sealed class ControlService {
    public string GetCommandParameter(object control) {
        return (control as ICommandSource).CommandParameter.ToString();
    }
}