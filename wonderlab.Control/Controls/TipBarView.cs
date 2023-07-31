using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System;
using System.Collections.ObjectModel;
using wonderlab.control.Controls.Bar;

namespace wonderlab.control.Controls {
    public class TipBarView : ContentControl {
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
            base.OnApplyTemplate(e);
            Background = null;
        }

        public void Add(string title, string message) {
            var result = new MessageTipsBar() { 
                Title = title,
                Message = message,
                Time = DateTime.Now.ToString(@"HH\:mm")
            };
            App.Cache.Add(result);
        }

        public ObservableCollection<MessageTipsBar> Items { get => GetValue(ItemsProperty); set => SetValue(ItemsProperty, value); }

        public static readonly StyledProperty<ObservableCollection<MessageTipsBar>> ItemsProperty =
            AvaloniaProperty.Register<TipBarView, ObservableCollection<MessageTipsBar>>(nameof(Items), App.Cache);
    }
}
