using Avalonia.Controls;
using Avalonia;
using Avalonia.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media;
using System.Reflection.Metadata;
using Avalonia.Controls.Metadata;
using Avalonia.Layout;
using System.Windows.Input;
using System.Collections;
using Avalonia.Collections;

namespace WonderLab.Views.Controls {
    [PseudoClasses(":fullscreen")]
    public class NavigationView : TemplatedControl {
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

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
            base.OnPropertyChanged(change);
            
            if (change.Property == IsFullScreenProperty) {
                UpdatePseudoClasses((bool)change.NewValue!);
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
