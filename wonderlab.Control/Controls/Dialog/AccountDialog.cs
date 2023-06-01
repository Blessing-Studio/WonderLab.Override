using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using System;
using System.Collections;
using System.Windows.Input;
using wonderlab.control.Animation;
using wonderlab.control.Interface;

namespace wonderlab.control.Controls.Dialog {
    public class AccountDialog : ContentControl, IDialog {
        private Button SelectButton = null!;

        private Button CloseButton = null!;

        private Border BackgroundBorder = null!;

        private Border DialogContent = null!;

        private ListBox AccountListBox = null!;

        public static object SelectedAccount { get; set; } = null!;

        public ICommand SelectedCommand { get => GetValue(SelectedCommandProperty); set => SetValue(SelectedCommandProperty, value); }

        public IEnumerable Accounts { get => GetValue(AccountsProperty); set => SetValue(AccountsProperty, value); }

        //public object SelectedAccount { get => GetValue(SelectedAccountProperty); set => SetValue(SelectedAccountProperty, value); }

        public static readonly StyledProperty<ICommand> SelectedCommandProperty =
            AvaloniaProperty.Register<AccountDialog, ICommand>(nameof(SelectedCommand));

        public static readonly StyledProperty<IEnumerable> AccountsProperty =
            AvaloniaProperty.Register<AccountDialog, IEnumerable>(nameof(Accounts));

        //public static readonly StyledProperty<object> SelectedAccountProperty =
        //    AvaloniaProperty.Register<AccountDialog, object>(nameof(SelectedAccount));

        public void HideDialog() {
            SelectButton.IsEnabled = false;
            BackgroundBorder.IsHitTestVisible = false;
            DialogContent.IsHitTestVisible = false;
            OpacityChangeAnimation animation = new(true);
            animation.RunAnimation(BackgroundBorder);
            animation.RunAnimation(DialogContent);
        }

        public void ShowDialog() {
            SelectButton.IsEnabled = false;
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
            AccountListBox = e.NameScope.Find<ListBox>("AccountListBox")!;
            AccountListBox.SelectionChanged += OnSelectionChanged;

            SelectButton = e.NameScope.Find<Button>("SelectButton")!;
            SelectButton!.Click += (_, _) => {
                HideDialog();
            };
            SelectButton.IsEnabled = false;

            CloseButton.Click += OnCloseButtonClick;

            BackgroundBorder.PointerPressed += (_, args) => {
                Manager.Current.BeginMoveDrag(args);
            };

            HideDialog();
        }

        //前端绑定不知道为啥不管用了
        private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e) {
            SelectButton.IsEnabled = true;
            SelectedAccount = AccountListBox.SelectedItem!;
        }

        private void OnCloseButtonClick(object? sender, RoutedEventArgs e) {
            HideDialog();
        }
    }
}
