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
using System.Reflection.Metadata;

namespace WonderLab.Views.Controls {
    public class Card : TemplatedControl {
        public static readonly StyledProperty<string> HeaderProperty =
            AvaloniaProperty.Register<Card, string>(nameof(Header), "Hello Header!");

        public static readonly StyledProperty<object> ContentProperty =
            AvaloniaProperty.Register<Card, object>(nameof(Content));

        public string Header { 
            get => GetValue(HeaderProperty); 
            set => SetValue(HeaderProperty, value);
        }

        public object Content {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }
    }

    public class CardItem : ListBoxItem {

        public static readonly StyledProperty<bool> IsLineVisibleProperty =
            AvaloniaProperty.Register<CardItem, bool>(nameof(IsLineVisible), true);

        public static readonly StyledProperty<string> HeaderProperty =
            AvaloniaProperty.Register<CardItem, string>(nameof(Header), "Hello Header!");

        public string Header {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public bool IsLineVisible {
            get => GetValue(IsLineVisibleProperty);
            set => SetValue(IsLineVisibleProperty, value);
        }
    }
}
