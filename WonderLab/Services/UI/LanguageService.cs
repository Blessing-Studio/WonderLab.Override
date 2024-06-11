using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace WonderLab.Services.UI;

public sealed class LanguageService {
    private readonly LogService _logService;
    private ResourceDictionary _actualLanguage;
    private readonly string _basePath = "avares://Wonderlab/Assets/Languages/";

    public LanguageService(LogService logService) {
        _logService = logService;
        _actualLanguage = AvaloniaXamlLoader
            .Load(new($"{_basePath}zh-CN.axaml")) as ResourceDictionary;
    }

    public void SetLanguage(int languageIndex) {
        string languageXaml = languageIndex switch {
            0 => "zh-CN.axaml",
            1 => "zh-TW.axaml",
            2 => "en-US.axaml",
            3 => "ru-RU.axaml",
            _ => "zh-CN.axaml"
        };

        var newLanguage = AvaloniaXamlLoader
            .Load(new($"{_basePath}{languageXaml}")) as ResourceDictionary;

        Application.Current.Resources.MergedDictionaries.Remove(_actualLanguage);
        Application.Current.Resources.MergedDictionaries.Add(newLanguage);

        _actualLanguage = newLanguage;
        _logService.Info(nameof(LanguageService), $"当前语言文件：{languageXaml}");
    }

    public bool TryGetValue(string key, out string value) {
        if (_actualLanguage.TryGetValue(key, out var nc)) {
            value = nc.ToString();
            return true;
        }

        value = "Not Found";
        return false;
    }
}