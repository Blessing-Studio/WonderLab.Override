using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Styling;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using wonderlab.control.Animation;
using wonderlab.control.Interface;

namespace wonderlab.control.Controls.Dialog {
    public class InstallDialog : ContentControl, IDialog {
        private Border BackgroundBorder = null!;
        
        private Border DialogContent = null!;

        private ListBox CurrentLoaders = null!;

        private Border FirstPanel = null!;

        private Button GlobalTopButton = null!;

        private StackPanel FirstPanelContent = null!;

        private StackPanel GlobalTopContent = null!;

        public string SelectedLoader { get => GetValue(SelectedLoaderProperty); set => SetValue(SelectedLoaderProperty, value); }

        public ICommand InstallCommand { get => GetValue(InstallCommandProperty); set => SetValue(InstallCommandProperty, value); }

        public static readonly StyledProperty<string> SelectedLoaderProperty =
            AvaloniaProperty.Register<InstallDialog, string>(nameof(SelectedLoader), "Optifine");

        public static readonly StyledProperty<ICommand> InstallCommandProperty =
            AvaloniaProperty.Register<InstallDialog, ICommand>(nameof(InstallCommand));

        public void HideDialog() {
            BackgroundBorder.IsHitTestVisible = false;
            DialogContent.IsHitTestVisible = false;
            OpacityChangeAnimation animation = new(true);
            animation.RunAnimation(BackgroundBorder);
            animation.RunAnimation(DialogContent);
            
            FirstPanel.Height = 350;
            FirstPanel.CornerRadius = new(8);
            GlobalTopContent.Height = GlobalTopButton.Height = 0;
        }

        public async void ShowDialog() {
            BackgroundBorder.IsHitTestVisible = true;
            DialogContent.IsHitTestVisible = true;

            OpacityChangeAnimation animation = new(false) {
                RunValue = 0
            };
            animation.RunAnimation(BackgroundBorder);
            animation.RunAnimation(DialogContent);

            await Task.Delay(300);
            FirstPanelContent.Width = 120;
            FirstPanelContent.Height = 90;
            await Task.Delay(500);
            FirstPanelContent.Width = 0;
            FirstPanelContent.Height = 0;
            await Task.Delay(100);
            FirstPanel.CornerRadius = new(8, 8, 0, 0);
            FirstPanel.Height = 35;
            await Task.Delay(100);
            GlobalTopContent.Height = GlobalTopButton.Height = 20;
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
            base.OnApplyTemplate(e);

            FirstPanel = e.NameScope.Find<Border>("FirstPanel")!;
            DialogContent = e.NameScope.Find<Border>("DialogContent")!;
            CurrentLoaders = e.NameScope.Find<ListBox>("CurrentLoaders")!;
            GlobalTopButton = e.NameScope.Find<Button>("GlobalTopButton")!;
            BackgroundBorder = e.NameScope.Find<Border>("BackgroundBorder")!;
            GlobalTopContent = e.NameScope.Find<StackPanel>("GlobalTopContent")!;
            FirstPanelContent = e.NameScope.Find<StackPanel>("FirstPanelContent")!;

            CurrentLoaders.SelectionChanged += OnCurrentModLoaderSelectionChanged;
            GlobalTopButton.Click += HideAction;
            
            ShowDialog();
        }

        private void OnCurrentModLoaderSelectionChanged(object? sender, SelectionChangedEventArgs e) {
            SetValue(SelectedLoaderProperty, ((ListBoxItem)((ListBox)sender!).SelectedItem!).Tag!.ToString()!);
        }

        private void HideAction(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
            HideDialog();
        }
    }
}