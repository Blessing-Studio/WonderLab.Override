using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.control.Controls.Dialog;

namespace wonderlab.control.Controls.Buttons
{
    public class HyperlinkButton : Button {
        private TextBlock Text = null;

        public static readonly StyledProperty<string> LinkProperty =
            AvaloniaProperty.Register<MessageDialog, string>(nameof(Link), "https://corona.studio/");

        public string Link { get => GetValue(LinkProperty); set => SetValue(LinkProperty, value); }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {       
            base.OnApplyTemplate(e);

            Text = e.NameScope.Find<TextBlock>("Main")!;

            this.Click += OnClick;
            Text.PointerEntered += OnPointerEntered;
            Text.PointerExited += OnPointerExited;
        }

        private void OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
            Process.Start(new ProcessStartInfo(Link) {           
                UseShellExecute = true,
                Verb = "open"
            });
        }

        private void OnPointerExited(object? sender, Avalonia.Input.PointerEventArgs e) {
            if (!(Text is null)) {
                Text.TextDecorations = new();
            }
        }

        private void OnPointerEntered(object? sender, Avalonia.Input.PointerEventArgs e) {
            if (!(Text is null)) {
                Text.TextDecorations = TextDecorations.Underline;
            }
        }
    }
}
