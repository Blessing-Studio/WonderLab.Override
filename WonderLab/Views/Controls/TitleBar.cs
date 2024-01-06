using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;

namespace WonderLab.Views.Controls;

public class TitleBar : TemplatedControl {
    private Window _window = null!;

    public async void Close() {
        await App.StopHostAsync();
        if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            desktop.MainWindow!.Close();
        }
    }

    public void Minimized() {
        _window.WindowState = WindowState.Minimized;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);
        if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            _window = desktop.MainWindow!;
        }

        e.NameScope.Find<Button>("CloseButton")!.Click += (sender, args) => {
            Close();
        };

        e.NameScope.Find<Button>("MinimizedButton")!.Click += (sender, args) => {
            Minimized();
        };

        this.PointerPressed += (_, args) => {
            _window.BeginMoveDrag(args);
        };
    }
}