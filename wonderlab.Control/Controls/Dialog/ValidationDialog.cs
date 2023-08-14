using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using wonderlab.control.Animation;
using wonderlab.control.Interface;

namespace wonderlab.control.Controls.Dialog {
    public class ValidationDialog : TemplatedControl, IDialog {
        public enum ValidationTypes {
            Offline,
            Yggdrasil,
            Microsoft
        }
        
        private Grid CodePanel = null!;

        private TextBox YggdrasilUrl = null!;

        private TextBox YggdrasilPassword = null!;

        private TextBox YggdrasilName = null!;

        private Grid WritePanel = null!;

        private Grid WritePanelButtons = null!;

        private Grid CodePanelButtons = null!;

        private Border Content = null!;

        private Border BackgroundBorder = null!;

        public ValidationTypes ValidationType { get => GetValue(ValidationTypeProperty); set => SetValue(ValidationTypeProperty, value); }

        public bool IsCodeLoading { get => GetValue(IsCodeLoadingProperty); set => SetValue(IsCodeLoadingProperty, value); }

        public bool HasGame { get => GetValue(HasGameProperty); set => SetValue(HasGameProperty, value); }

        public ICommand ValidationWriteCommand { get => GetValue(ValidationWriteCommandProperty); set => SetValue(ValidationWriteCommandProperty, value); }

        public ICommand ValidationMicrosoftCommand { get => GetValue(ValidationMicrosoftCommandProperty); set => SetValue(ValidationMicrosoftCommandProperty, value); }

        public ICommand CancelCommand { get => GetValue(CancelCommandProperty); set => SetValue(CancelCommandProperty, value); }

        public string DeviceCode { get => GetValue(DeviceCodeProperty); set => SetValue(DeviceCodeProperty, value); }

        public string YggdrasilUri { get => GetValue(YggdrasilUriProperty); set => SetValue(YggdrasilUriProperty, value); }

        public string Email { get => GetValue(EmailProperty); set => SetValue(EmailProperty, value); }

        public string Password { get => GetValue(PasswordProperty); set => SetValue(PasswordProperty, value); }
        
        public static readonly StyledProperty<string> YggdrasilUriProperty =
            AvaloniaProperty.Register<ValidationDialog, string>(nameof(YggdrasilUri));

        public static readonly StyledProperty<string> EmailProperty =
            AvaloniaProperty.Register<ValidationDialog, string>(nameof(Email));

        public static readonly StyledProperty<string> PasswordProperty =
            AvaloniaProperty.Register<ValidationDialog, string>(nameof(Password));

        public static readonly StyledProperty<string> DeviceCodeProperty =
            AvaloniaProperty.Register<ValidationDialog, string>(nameof(DeviceCode));

        public static readonly StyledProperty<ICommand> ValidationWriteCommandProperty =
            AvaloniaProperty.Register<ValidationDialog, ICommand>(nameof(ValidationWriteCommand));

        public static readonly StyledProperty<ICommand> ValidationMicrosoftCommandProperty =
            AvaloniaProperty.Register<ValidationDialog, ICommand>(nameof(ValidationMicrosoftCommand));

        public static readonly StyledProperty<ICommand> CancelCommandProperty =
            AvaloniaProperty.Register<ValidationDialog, ICommand>(nameof(CancelCommand));

        public static readonly StyledProperty<bool> IsCodeLoadingProperty =
            AvaloniaProperty.Register<ValidationDialog, bool>(nameof(IsCodeLoading), false);

        public static readonly StyledProperty<bool> HasGameProperty =
            AvaloniaProperty.Register<ValidationDialog, bool>(nameof(HasGame), false);

        public static readonly StyledProperty<ValidationTypes> ValidationTypeProperty =
            AvaloniaProperty.Register<ValidationDialog, ValidationTypes>(nameof(ValidationType), ValidationTypes.Offline);

        public void HideDialog() {
            BackgroundBorder.IsHitTestVisible = false;
            Content.IsHitTestVisible = false;

            Password = Email = YggdrasilUri = string.Empty;
            OpacityChangeAnimation animation = new(true);
            animation.RunAnimation(BackgroundBorder);
            animation.RunAnimation(Content);
        }

        public void ShowDialog() {
            BackgroundBorder.IsHitTestVisible = true;
            Content.IsHitTestVisible = true;

            OpacityChangeAnimation animation = new(false) {
                RunValue = 0
            };
            animation.RunAnimation(BackgroundBorder);
            animation.RunAnimation(Content);
        }

        public ValidationTypes ShowDialog(ValidationTypes types) {
            if (types is ValidationTypes.Offline) {
                WritePanel.IsVisible = true;
                IsCodeLoading = false;
                CodePanel.IsVisible = false;
                YggdrasilUrl.IsVisible = false;
                YggdrasilPassword.IsVisible = false;
                CodePanelButtons.IsVisible = false;
                WritePanelButtons.IsVisible = true;
                ValidationType = ValidationTypes.Offline;
            } else if (types is ValidationTypes.Yggdrasil) {
                WritePanel.IsVisible = true;
                IsCodeLoading = false;
                CodePanel.IsVisible = false;
                CodePanelButtons.IsVisible = false;
                WritePanelButtons.IsVisible = true;
                YggdrasilUrl.IsVisible = YggdrasilPassword.IsVisible = true;
                ValidationType = ValidationTypes.Yggdrasil;
            } else {
                WritePanel.IsVisible = false;
                IsCodeLoading = true;
                WritePanelButtons.IsVisible = false;
                ValidationType = ValidationTypes.Microsoft;
            }

            YggdrasilUrl.AddHandler(DragDrop.DropEvent, (o, args) => {
                if (!string.IsNullOrEmpty(args.Data.GetText())) {
                    SetValue(YggdrasilUriProperty!, args.Data.GetText()!.Replace("authlib-injector:yggdrasil-server:", 
                        string.Empty).Replace("%2F", "/").Replace("%3A", ":"));
                }
            });

            ShowDialog();
            return ValidationType;
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
            base.OnApplyTemplate(e);
            
            CodePanel = e.NameScope.Find<Grid>("CodePanel")!;
            WritePanel = e.NameScope.Find<Grid>("WritePanel")!;
            Content = e.NameScope.Find<Border>("DialogContent")!;
            YggdrasilUrl = e.NameScope.Find<TextBox>("YggdrasilUrl")!;
            CodePanelButtons = e.NameScope.Find<Grid>("CodePanelButtons")!;
            BackgroundBorder = e.NameScope.Find<Border>("BackgroundBorder")!;
            WritePanelButtons = e.NameScope.Find<Grid>("WritePanelButtons")!;
            YggdrasilPassword = e.NameScope.Find<TextBox>("YggdrasilPassword")!;
            YggdrasilName = e.NameScope.Find<TextBox>("Name")!;
            HideDialog();

            this.PropertyChanged += OnPropertyChanged;
            YggdrasilName.TextChanged += (_, _) => {
                Email = YggdrasilName.Text!;
            };

            YggdrasilUrl.TextChanged += (_, _) => {
                YggdrasilUri = YggdrasilUrl.Text!;
            };

            YggdrasilPassword.TextChanged += (_, _) => {
                Password = YggdrasilPassword.Text!;
            };
        }

        private void OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e) {
            if (e.Property == IsCodeLoadingProperty && !WritePanel.IsVisible) {
                CodePanel.IsVisible = !IsCodeLoading;
                CodePanelButtons.IsVisible = !IsCodeLoading;
            }

            Trace.WriteLine(e.Property.Name);
        }
    }
}
