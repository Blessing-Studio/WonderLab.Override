using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
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
    public class UpdateDialog : ContentControl, IDialog {
        private Border BackgroundBorder = null!;

        private Border DialogContent = null!;

        private Button CloseButton = null!;

        public object? Message { get => GetValue(MessageProperty); set => SetValue(MessageProperty!, value); }

        public string? Author { get => GetValue(AuthorProperty); set => SetValue(AuthorProperty!, value); }

        public ICommand UpdateButtonCommand { get => GetValue(UpdateButtonCommandProperty); set => SetValue(UpdateButtonCommandProperty!, value); }

        public bool Update { get => GetValue(UpdateProperty); set => SetValue(UpdateProperty!, value); }

        public double UpdateProgress { get => GetValue(UpdateProgressProperty); set => SetValue(UpdateProgressProperty, value); }

        public bool IsUpdate { get; set; } = false;

        public static readonly StyledProperty<string> AuthorProperty =
            AvaloniaProperty.Register<UpdateDialog, string>(nameof(Author), "");

        public static readonly StyledProperty<object> MessageProperty =
            AvaloniaProperty.Register<UpdateDialog, object>(nameof(Message), "");

        public static readonly StyledProperty<bool> UpdateProperty =
            AvaloniaProperty.Register<UpdateDialog, bool>(nameof(Message), false);

        public static readonly StyledProperty<double> UpdateProgressProperty =
            AvaloniaProperty.Register<UpdateDialog, double>(nameof(UpdateProgress), 0.0);

        public static readonly StyledProperty<ICommand> UpdateButtonCommandProperty =
            AvaloniaProperty.Register<UpdateDialog, ICommand>(nameof(UpdateButtonCommand));

        public void HideDialog() {
            BackgroundBorder.IsHitTestVisible = false;
            DialogContent.IsHitTestVisible = false;
            OpacityChangeAnimation animation = new(true);
            animation.RunAnimation(BackgroundBorder);
            animation.RunAnimation(DialogContent);
        }

        public void ShowDialog() {
            BackgroundBorder.IsHitTestVisible = true;
            DialogContent.IsHitTestVisible = true;

            OpacityChangeAnimation animation = new(false) {
                RunValue = 0
            };
            animation.RunAnimation(BackgroundBorder);
            animation.RunAnimation(DialogContent);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
            base.OnApplyTemplate(e);

            BackgroundBorder = e.NameScope.Find<Border>("BackgroundBorder")!;
            DialogContent = e.NameScope.Find<Border>("DialogContent")!;
            CloseButton = e.NameScope.Find<Button>("CloseButton")!;
            CloseButton!.Click += (_, _) => {
                HideDialog();
            };

            e.NameScope.Find<Button>("CustomButton")!.Click += (_, _) => {
                Update = true;
            };

            BackgroundBorder.PointerPressed += (_, args) => {
                Manager.Current.BeginMoveDrag(args);
            };

            if (!IsUpdate) {
                HideDialog();
            }
        }
    }
}
