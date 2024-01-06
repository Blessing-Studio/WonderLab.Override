using System.Linq;
using System.Reflection;
using System.ComponentModel;
using WonderLab.Classes.Managers;
using System.Collections.Generic;
using WonderLab.Classes.Attributes;
using CommunityToolkit.Mvvm.ComponentModel;
using SixLabors.ImageSharp.ColorSpaces;

namespace WonderLab.ViewModels;

public class ViewModelBase : ObservableObject {
    private DataManager _configDataManager;
    private Dictionary<string, FieldInfo> _configFields;

    public ViewModelBase() { }

    public ViewModelBase(DataManager manager) {
        BackgroundWorker worker = new();
        worker.DoWork += (sender, args) => {
            var properties = GetType()
                .GetFields()
                .Where(x => x.GetCustomAttribute<BindToConfigAttribute>() is not null)
                .ToDictionary(x => x.Name, x => x);

            var config = manager.Config;
            if (properties.Any()) {
                PropertyChanged += OnPropertyChanged;
            }

            _configFields = properties;
            _configDataManager = manager;
            InitDataToView();
        };
        
        worker.RunWorkerAsync();
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (_configFields.TryGetValue(e.PropertyName, out var fieldA)) {
            var propertyB = GetPropertyB(fieldA);

            propertyB!.SetValue(_configDataManager.Config,
                fieldA.GetValue(this));
        }
    }

    private void InitDataToView() {
        foreach (var item in _configFields) {
            var fieldA = item.Value;
            var propertyB = GetPropertyB(fieldA);

            fieldA.SetValue(this, propertyB!
                .GetValue(_configDataManager.Config));
        }
    }

    private PropertyInfo GetPropertyB(FieldInfo fieldA) => _configDataManager
        .Config
        .GetType()
        .GetProperty(fieldA
        .GetCustomAttribute<BindToConfigAttribute>()!
        .ConfigName)!;
}
