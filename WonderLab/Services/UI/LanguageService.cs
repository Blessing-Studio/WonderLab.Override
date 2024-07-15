using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Logging;

namespace WonderLab.Services.UI;

public sealed class LanguageService {
    private readonly ILogger<LanguageService> _logger;
    private ResourceDictionary _actualLanguage;
    private readonly string _basePath = "avares://Wonderlab/Assets/Languages/";

    public LanguageService(ILogger<LanguageService> logger) {
        _logger = logger;
        _actualLanguage = AvaloniaXamlLoader
            .Load(new($"{_basePath}zh-CN.axaml")) as ResourceDictionary;
    }

    public void SetLanguage(int languageIndex) {
        string languageXaml = languageIndex switch {
            0 => "zh-CN.axaml",
            1 => "zh-TW.axaml",
            2 => "zh-lzh.axaml",
            3 => "en-US.axaml",
            4 => "ru-RU.axaml",
            5 => "ja-JP.axaml",
            _ => "zh-CN.axaml"
        };

        var newLanguage = AvaloniaXamlLoader
            .Load(new($"{_basePath}{languageXaml}")) as ResourceDictionary;

        Application.Current.Resources.MergedDictionaries.Remove(_actualLanguage);
        Application.Current.Resources.MergedDictionaries.Add(newLanguage);

        _actualLanguage = newLanguage;
        _logger.LogInformation("当前语言文件：{LanguageXaml}", languageXaml);
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