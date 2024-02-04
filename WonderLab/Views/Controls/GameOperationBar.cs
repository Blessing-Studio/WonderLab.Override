using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WonderLab.Views.Controls;

public class GameOperationBar : TemplatedControl
{
    private Button _button;
    private TextBlock _title;
    private ContentControl _contentControl;

    private double _oldWidthValue = 155;
    private const int MAX_GAMEBAR_WIDTH = 645;
    private const int MAX_GAMEBAR_HEIGHT = 370;
    private CancellationTokenSource _cancellationTokenSource = new();

    public static GameOperationBar Scope { get; set; }

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public ICommand OpenCommand
    {
        get => GetValue(OpenCommandProperty);
        set => SetValue(OpenCommandProperty, value);
    }

    public object Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string SubTitle
    {
        get => GetValue(SubTitleProperty);
        set => SetValue(SubTitleProperty, value);
    }

    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<GameOperationBar, string>(nameof(Title), "Test title");

    public static readonly StyledProperty<string> SubTitleProperty =
        AvaloniaProperty.Register<GameOperationBar, string>(nameof(SubTitle), "Some message");

    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<GameOperationBar, bool>(nameof(IsOpen), false);

    public static readonly StyledProperty<object> ContentProperty =
        AvaloniaProperty.Register<GameOperationBar, object>(nameof(Content));

    public static readonly StyledProperty<ICommand> OpenCommandProperty =
        AvaloniaProperty.Register<GameOperationBar, ICommand>(nameof(OpenCommand));

    public GameOperationBar()
    {
        Scope = this;
    }

    public async Task CollapseInterface()
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            Height = 85;
            Width = _oldWidthValue;
            _button.Content = "展开界面";
            IsOpen = false;
            _contentControl.Opacity = 0;
        }, DispatcherPriority.Render, _cancellationTokenSource.Token);
    }

    private async Task ExpandInterface()
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            Width = MAX_GAMEBAR_WIDTH;
            Height = MAX_GAMEBAR_HEIGHT;
            _button.Content = "收起界面";
            IsOpen = true;
        }, DispatcherPriority.Render, _cancellationTokenSource.Token);

        await Task.Delay(300, _cancellationTokenSource.Token).ContinueWith(async x =>
        {
            if (x.IsCompletedSuccessfully)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    _contentControl.Opacity = 1;
                    OpenCommand.Execute(this);
                }, DispatcherPriority.Normal);

            }
        });
    }

    private async void OnClick(object? sender, RoutedEventArgs e)
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            if (IsOpen)
            {
                await CollapseInterface();
            }
            else
            {
                await ExpandInterface();
            }
        }
        catch (TaskCanceledException) { }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _title = e.NameScope.Find<TextBlock>("Title")!;
        _button = e.NameScope.Find<Button>("ControlButton")!;
        _contentControl = e.NameScope.Find<ContentControl>("contentControl")!;
        _button.Click += OnClick;
    }

    protected override async void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == TitleProperty)
        {
            var formattedText = new FormattedText(
                change.GetNewValue<string>(),
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(_title.FontFamily, _title.FontStyle, _title.FontWeight),
                _title.FontSize,
                Brushes.Black
            );

            var textWidth = formattedText.Width;
            _oldWidthValue = Width = textWidth > 140
                ? 15 + textWidth
                : 155;
        }
    }
}