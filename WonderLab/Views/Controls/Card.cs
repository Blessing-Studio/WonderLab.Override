using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls.Primitives;
using System.Windows.Input;

namespace WonderLab.Views.Controls {
    public class Card : TemplatedControl {
        public static readonly StyledProperty<IEnumerable> CardItemsProperty =
            AvaloniaProperty.Register<NavigationView, IEnumerable>(nameof(CardItems), new AvaloniaList<NavigationViewItem>());

        public static readonly StyledProperty<string> HeaderProperty =
            AvaloniaProperty.Register<FontIcon, string>(nameof(Header), "Hello Header!");

        public string Header { 
            get => GetValue(HeaderProperty); 
            set => SetValue(HeaderProperty, value);
        }

        public IEnumerable CardItems {
            get => GetValue(CardItemsProperty);
            set => SetValue(CardItemsProperty, value);
        }
    }

    public class CardItem : ListBoxItem {
        public static readonly StyledProperty<string> HeaderProperty =
            AvaloniaProperty.Register<FontIcon, string>(nameof(Header), "Hello Header!");

        public string Header {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }
    }
}
