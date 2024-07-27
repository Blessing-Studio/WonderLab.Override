using Avalonia;
using Avalonia.Media;
using Avalonia.Metadata;
using System.Windows.Input;
using Avalonia.Controls.Primitives;

namespace WonderLab.Views.Controls;

public sealed class ImageCard : TemplatedControl {
    public static readonly StyledProperty<IImage> SourceProperty =
        AvaloniaProperty.Register<ImageCard, IImage>(nameof(Source));

    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<ImageCard, string>(nameof(Source));

    public static readonly StyledProperty<string> DescriptionProperty =
        AvaloniaProperty.Register<ImageCard, string>(nameof(Source));

    public static readonly StyledProperty<ICommand> CommandProperty =
        AvaloniaProperty.Register<ImageCard, ICommand>(nameof(Command));

    [Content]
    public IImage Source {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public string Title {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Description {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public ICommand Command {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
}