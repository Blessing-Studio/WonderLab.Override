using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Threading;
using WonderLab.Classes.Datas.ViewData;
using WonderLab.Utilities;

namespace WonderLab.Views.Controls;

/// <summary>
/// 游戏实体管理面板控件
/// </summary>
public sealed class GameManagerPanel : ContentControl {
    private Grid _contentPanel = default!;
    private ListBox _gameListBox = default!;
    private Button _openPaneButton = default!;
    private TextBlock _titleTextBlock = default!;
    private TextBlock _subTitleTextBlock = default!;
    private Rect _rectCache = new(0, 0, 155, 85);
    private CancellationTokenSource _cancellationTokenSource = new();
    private readonly Rect _maxRect = new(0, 0, 645, 370);
    
    public bool IsPaneOpen {
        get => GetValue(IsPaneOpenProperty); 
        set => SetValue(IsPaneOpenProperty, value);
    }

    public ICommand OpenCommand { 
        get => GetValue(OpenCommandProperty);
        set => SetValue(OpenCommandProperty, value);
    }
    
    public GameViewData SelectedGame {
        get => GetValue(SelectedGameProperty);
        set => SetValue(SelectedGameProperty, value);
    }

    public IEnumerable<GameViewData> GameEntries {
        get => GetValue(GameEntriesProperty);
        set => SetValue(GameEntriesProperty, value);
    }
    
    public static readonly StyledProperty<bool> IsPaneOpenProperty =
        AvaloniaProperty.Register<GameManagerPanel, bool>(nameof(IsPaneOpen), false);

    public static readonly StyledProperty<GameViewData> SelectedGameProperty =
        AvaloniaProperty.Register<GameManagerPanel, GameViewData>(nameof(SelectedGame));
    
    public static readonly StyledProperty<ICommand> OpenCommandProperty =
        AvaloniaProperty.Register<GameManagerPanel, ICommand>(nameof(OpenCommand));
    
    public static readonly StyledProperty<IEnumerable<GameViewData>> GameEntriesProperty =
        AvaloniaProperty.Register<GameManagerPanel, IEnumerable<GameViewData>>(nameof(GameEntries),
            new AvaloniaList<GameViewData>());

    private void OnOpenPaneButtonClick(object? sender, RoutedEventArgs e) {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new CancellationTokenSource();
        
        if (!IsPaneOpen) { 
            OpenPaneAsync();
            return;
        }

        ClosePane();
        return;

        async void OpenPaneAsync() { 
            Dispatcher.UIThread.Post(() => {
                Width = _maxRect.Width;
                Height = _maxRect.Height;
                _openPaneButton.Content = "收起界面";
                IsPaneOpen = true;
            });

            await Task.Delay(390, _cancellationTokenSource.Token).ContinueWith(x => {
                if (x.IsCompletedSuccessfully) {
                    Dispatcher.UIThread.Post(() => _contentPanel.Opacity = 1);
                }
            });
        }

        void ClosePane() {
            Dispatcher.UIThread.Post(() => {
                _contentPanel.Opacity = 0;
                Height = 85;
                Width = _rectCache.Width;
                _openPaneButton.Content = "展开界面";
                IsPaneOpen = false;
            });
        }
    }

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);
        if (SelectedGame != null) {
            _rectCache = MathUtil.CalculateText(SelectedGame.Entry.Id, _titleTextBlock);
            Height = 85;
            IsPaneOpen = false;
            Width = _rectCache.Width;
            _contentPanel.Opacity = 0;
            _titleTextBlock.Text = SelectedGame.Entry.Id;
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        _gameListBox = e.NameScope.Get<ListBox>("GameListBox");
        _contentPanel = e.NameScope.Get<Grid>("ContentPanel");
        _openPaneButton = e.NameScope.Get<Button>("OpenPaneButton");
        _titleTextBlock = e.NameScope.Get<TextBlock>("TitleTextBlock");
        _subTitleTextBlock = e.NameScope.Get<TextBlock>("SubTitleTextBlock");
        
        _titleTextBlock.Text = "未选择游戏";
        _openPaneButton.Click += OnOpenPaneButtonClick;
        _gameListBox.SelectionChanged += OnGameListBoxSelectionChanged;
    }

    private void OnGameListBoxSelectionChanged(object? sender, SelectionChangedEventArgs e) {
        SelectedGame = _gameListBox.SelectedItem as GameViewData ?? SelectedGame;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (change.Property == SelectedGameProperty && _titleTextBlock != null) {
            string gameId = change.GetNewValue<GameViewData>().Entry.Id;
            _rectCache = MathUtil.CalculateText(gameId, _titleTextBlock);
            Dispatcher.UIThread.Post(() => {
                Height = 85;
                IsPaneOpen = false;
                Width = _rectCache.Width;
                _contentPanel.Opacity = 0;
                _titleTextBlock.Text = gameId;
            });
        }
    }
}