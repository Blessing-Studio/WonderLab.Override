using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Chrome;
using Avalonia.Controls.Primitives;
using System.Threading.Tasks;
using System.Windows.Input;
using wonderlab.control.Controls.Dialog;

namespace wonderlab.control.Controls.Bar {
    public class TitleBar : TemplatedControl {
        private Border top;

        public TitleBar() {
        }

        public TitleBar(Window window) {
            Window = window;
        }

        public static readonly StyledProperty<ICommand> GoBackCommandProperty =
            AvaloniaProperty.Register<TitleBar, ICommand>(nameof(GoBackCommand));

        public static readonly StyledProperty<double> TitleWidthProperty =
            AvaloniaProperty.Register<TitleBar, double>(nameof(GoBackCommand));

        public static readonly StyledProperty<string> TitleProperty =
            AvaloniaProperty.Register<TitleBar, string>(nameof(Title));

        public ICommand GoBackCommand { get => GetValue(GoBackCommandProperty); set => SetValue(GoBackCommandProperty, value); }

        public double TitleWidth { get => GetValue(TitleWidthProperty); set => SetValue(TitleWidthProperty, value); }

        public string Title { get => GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

        public static Window Window { get; set; }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
            base.OnApplyTemplate(e);
            top = e.NameScope.Find<Border>("TopBar");
            RunAnimation();

            e.NameScope.Find<Button>("close").Click += (_, _) => CloseAction();
            e.NameScope.Find<Button>("mini").Click += (_, _) => MiniAction();
        }

        private void MiniAction() {
            Window.WindowState = WindowState.Minimized;
        }

        private void CloseAction() {
            Window.Close();
        }

        private async void RunAnimation() {
            await Task.Delay(100);
            top.Margin = new(0);
        }
    }
}
