using System;
using ReactiveUI;
using System.ComponentModel;
using System.Reflection;
using WonderLab.Classes.Attributes;
using System.Linq;
using WonderLab.Classes.Managers;
using System.Collections.Generic;
using System.Diagnostics;

namespace WonderLab.ViewModels;

public class ViewModelBase : ReactiveObject {
    private Dictionary<string, PropertyInfo> _configPropertys;
    private DataManager _configDataManager;

    public ViewModelBase() { }

    public ViewModelBase(DataManager manager) {
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
        InitDataToView();
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (_configPropertys.TryGetValue(e.PropertyName, out var propertyA)) {
            var propertyB = GetPropertyB(propertyA);

            propertyB!.SetValue(_configDataManager.Config, 
                propertyA.GetValue(this));
        }
    }

    private void InitDataToView() {
        foreach (var item in _configPropertys) {
            var propertyA = item.Value;
            var propertyB = GetPropertyB(propertyA);

            propertyA.SetValue(this, propertyB!
                .GetValue(_configDataManager.Config));
        }
    }

    private PropertyInfo GetPropertyB(PropertyInfo propertyA) => _configDataManager
        .Config
        .GetType()
        .GetProperty(propertyA
        .GetCustomAttribute<BindToConfigAttribute>()!
        .ConfigName)!;
}
