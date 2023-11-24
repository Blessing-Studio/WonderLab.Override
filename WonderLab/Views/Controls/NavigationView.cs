using Avalonia;
using Avalonia.Controls;
using System.Collections;
using System.Windows.Input;
using Avalonia.Collections;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Avalonia.Controls.Presenters;
using System.Threading;
using Avalonia.Interactivity;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WonderLab.Views.Controls {
    [PseudoClasses(":fullscreen")]
    public class NavigationView : TemplatedControl {
        private ContentPresenter? _contentPresenter;

        private CancellationTokenSource _token = new();

        public static readonly StyledProperty<IEnumerable> MenuItemsProperty =
            AvaloniaProperty.Register<NavigationView, IEnumerable>(nameof(MenuItems),new AvaloniaList<NavigationViewItem>());

        public static readonly StyledProperty<object> ContentProperty =
            AvaloniaProperty.Register<NavigationView, object>(nameof(Content));

        public static readonly StyledProperty<bool> IsFullScreenProperty =
            AvaloniaProperty.Register<NavigationView, bool>(nameof(IsFullScreen));

        public IEnumerable MenuItems
        {
            get
            {
                return GetValue(MenuItemsProperty);
            }
            set
            {
                SetValue(MenuItemsProperty, value);
            }
        }

        public object Content
        {
            get
            {
                return GetValue(ContentProperty);
            }
            set
            {
                SetValue(ContentProperty, value);
            }
        }

        public bool IsFullScreen
        {
            get
            {
                return GetValue(IsFullScreenProperty);
            }
            set
            {
                SetValue(IsFullScreenProperty, value);
            }
        }

        public NavigationView() {
            UpdatePseudoClasses(false);
        }

        public void UpdatePseudoClasses(bool? isFullScreen) {
            if (isFullScreen.HasValue) {
                PseudoClasses.Set(":fullscreen", isFullScreen.Value);
            }
        }
        
        private async void RunPageTransitionAnimation() {
            if (_contentPresenter == null) {
                return;
            }

            await Dispatcher.UIThread.InvokeAsync(async () => {
                _contentPresenter!.Opacity = 0;
                await Task.Delay(350);
            }).ContinueWith(async task => {
                await Dispatcher.UIThread.InvokeAsync(() => {
                    _contentPresenter.Content = Content;
                    _contentPresenter!.Opacity = 1;
                }, DispatcherPriority.Render);
            });
        }

        protected override void OnLoaded(RoutedEventArgs e) {
            base.OnLoaded(e);
            _contentPresenter.Content = Content;
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
            base.OnApplyTemplate(e);

            _contentPresenter = e.NameScope
                .Find<ContentPresenter>("Content");
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
            base.OnPropertyChanged(change);
            
            if (change.Property == IsFullScreenProperty) {
                UpdatePseudoClasses((bool)change.NewValue!);
            }

            if (change.Property == ContentProperty) {
                RunPageTransitionAnimation();
            }
        }
    }

    public class NavigationViewItem : ListBoxItem {
        public static readonly StyledProperty<string> IconProperty =
            AvaloniaProperty.Register<NavigationViewItem, string>(nameof(Icon));

        public static readonly StyledProperty<ICommand> CommandProperty =
            AvaloniaProperty.Register<NavigationViewItem, ICommand>(nameof(Command));

        public string Icon
        {
            get
            {
                return GetValue(IconProperty);
            }
            set
            {
                SetValue(IconProperty, value);
            }
        }

        public ICommand Command
        {
            get
            {
                return GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }

        public NavigationViewItem() {
            //UpdatePseudoClasses(IsSelected);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
            base.OnApplyTemplate(e);

            e.NameScope.Find<Button>("MainLayout")!.Click += (sender, args) => {
                IsSelected = !IsSelected;
            };
        }
    }
}
