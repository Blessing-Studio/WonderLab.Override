using System;
using CommunityToolkit.Mvvm.Messaging;
using WonderLab.Classes.Models.Messaging;

namespace WonderLab.Services.UI;

public class NavigationService {
    private readonly string _baseNameSpace = "WonderLab.Views.Pages.";

    public bool Navigation(string key, bool isChildrenPage = false) {
        var page = App.ServiceProvider
            .GetService(Type.GetType($"{_baseNameSpace}{key}")!);

        if (page is null) {
            return false;
        }
        
        WeakReferenceMessenger.Default.Send(new PageMessage {
            Page = page,
            PageName = key,
            IsChildrenPage = isChildrenPage
        });

        return true;
    }
}