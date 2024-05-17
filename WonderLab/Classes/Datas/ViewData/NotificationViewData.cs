using CommunityToolkit.Mvvm.ComponentModel;

namespace WonderLab.Classes.Datas.ViewData;

public sealed partial class NotificationViewData : ObservableObject {
    [ObservableProperty] private string _time;
    [ObservableProperty] private string _title;
    [ObservableProperty] private string _content;
}