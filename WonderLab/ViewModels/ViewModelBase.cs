using System;
using ReactiveUI;
using System.ComponentModel;
using System.Reflection;
using WonderLab.Classes.Attributes;
using System.Linq;
using WonderLab.Classes.Managers;
using System.Collections.Generic;

namespace WonderLab.ViewModels;

public class ViewModelBase : ReactiveObject {
    private Dictionary<string, PropertyInfo> _configPropertys;
    private ConfigDataManager _configDataManager;

    public ViewModelBase() { }

    public ViewModelBase(ConfigDataManager manager) {
        var properties = GetType()
            .GetProperties()
            .Where(x => x.GetCustomAttribute<BindToConfigAttribute>() is not null)
            .ToDictionary(x => x.Name, x => x);

        var config = manager.Config;

        if (properties.Any()) {
            PropertyChanged += OnPropertyChanged;
        }

        _configDataManager = manager;
        _configPropertys = properties;
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (_configPropertys.TryGetValue(e.PropertyName, out var propertyA)) {
            var propertyB = _configDataManager.Config
                .GetType()
                .GetProperty(propertyA
                .GetCustomAttribute<BindToConfigAttribute>()!
                .ConfigName);

            propertyB!.SetValue(_configDataManager.Config, 
                propertyA.GetValue(this));
        }
    }
}
