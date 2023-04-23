using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.control.Controls.Dialog;
using wonderlab.control.Controls.Dialog.Events;

namespace wonderlab.control.Controls
{
    /// <summary>
    /// 轮播图控件
    /// </summary>
    public class FilpView : TemplatedControl {
        public static readonly StyledProperty<ObservableCollection<IImage>> SourceProperty =
            AvaloniaProperty.Register<FilpView, ObservableCollection<IImage>>(nameof(Source), new());

        public static readonly StyledProperty<IImage> CurrentProperty =
            AvaloniaProperty.Register<FilpView, IImage>(nameof(Current));

        public ObservableCollection<IImage> Source { get => GetValue(SourceProperty); set => SetValue(SourceProperty, value); }

        public IImage Current { get => GetValue(CurrentProperty); set => SetValue(CurrentProperty, value); }

        public Button MoveToLeftButton, MoveToRightButton;
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {       
            base.OnApplyTemplate(e);

            Current = Source is null ? null : Source.First();

            MoveToLeftButton = e.NameScope.Find<Button>("MoveToLeftButton");
            MoveToRightButton = e.NameScope.Find<Button>("MoveToRightButton");

            MoveToLeftButton.Click += MoveToLeftButton_Click;
            MoveToRightButton.Click += MoveToRightButton_Click;
            MoveToLeftButton.IsVisible = false;
        }

        private void MoveToRightButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)  {      
            var result = Source.IndexOf(Current);
            if (result != -1 && result + 1 < Source.Count) {           
                Current = Source[result + 1];

                MoveToRightButton.IsVisible = !(Current == Source.Last());
                MoveToLeftButton.IsVisible = true;
            }
        }

        private void MoveToLeftButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
            var result = Source.IndexOf(Current);
            if(result != -1 && result > 0) {
                Current = Source[result - 1];
                MoveToLeftButton.IsVisible = !(Current == Source.First());
                MoveToRightButton.IsVisible = true;
            }
        }
    }
}
