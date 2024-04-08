using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace WonderLab.Services.UI;

public sealed class LanguageService {
    private ResourceDictionary _actualLanguage;

    private readonly SettingService _settingService;
    private readonly string _basePath = "avares://Wonderlab/Assets/Languages/";

    public LanguageService(SettingService settingService) {
        _settingService = settingService;

        _actualLanguage = AvaloniaXamlLoader
            .Load(new($"{_basePath}zh-CN.axaml")) as ResourceDictionary;
    }

    public void SetLanguage(int languageIndex) {
        string languageXaml = languageIndex switch {
            0 => "zh-CN.axaml",
            1 => "en-US.axaml",
            _ => "zh-CN.axaml"
        };

        var newLanguage = AvaloniaXamlLoader
            .Load(new($"{_basePath}{languageXaml}")) as ResourceDictionary;

        Application.Current.Resources.MergedDictionaries.Remove(_actualLanguage);
        Application.Current.Resources.MergedDictionaries.Add(newLanguage);

        _actualLanguage = newLanguage;
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