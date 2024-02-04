using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System.Collections;

namespace WonderLab.Views.Controls;

public class Card : TemplatedControl
{
    public static readonly StyledProperty<string> HeaderProperty =
        AvaloniaProperty.Register<Card, string>(nameof(Header), "Hello Header!");

    public static readonly StyledProperty<object> ContentProperty =
        AvaloniaProperty.Register<Card, object>(nameof(Content));

    public static readonly StyledProperty<IEnumerable> CardItemsProperty =
        AvaloniaProperty.Register<Card, IEnumerable>(nameof(CardItems), new AvaloniaList<CardItem>());

    public string Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public object Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public IEnumerable CardItems
    {
        get => GetValue(CardItemsProperty);
        set => SetValue(CardItemsProperty, value);
    }
}

public class CardItem : ListBoxItem
{
    public static readonly StyledProperty<bool> IsLineVisibleProperty =
        AvaloniaProperty.Register<CardItem, bool>(nameof(IsLineVisible), true);

    public static readonly StyledProperty<string> HeaderProperty =
        AvaloniaProperty.Register<CardItem, string>(nameof(Header), "Hello Header!");

    public string Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public bool IsLineVisible
    {
        get => GetValue(IsLineVisibleProperty);
        set => SetValue(IsLineVisibleProperty, value);
    }
}