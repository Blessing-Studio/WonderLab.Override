using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;
using Microsoft.VisualBasic;
using MinecaftOAuth.Authenticator;
using MinecraftLaunch.Modules.Installer;
using MinecraftLaunch.Modules.Models.Auth;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using wonderlab.Class.Models;
using wonderlab.Class.Utils;
using wonderlab.Class.ViewData;
using wonderlab.control.Animation;
using wonderlab.control.Controls.Bar;
using wonderlab.control.Controls.Dialog;
using wonderlab.control.Theme;
using wonderlab.ViewModels.Windows;
using wonderlab.Views.Pages;
using static System.Collections.Specialized.BitVector32;
using static wonderlab.control.Controls.Bar.MessageTipsBar;

namespace wonderlab
{
    public partial class MainWindow : Window
    {
        private double X, Y, WindowHeight, WindowWidth;
        public bool isDragging, IsOpen, IsUseDragging, CanParallax;
        public static MainWindowViewModel ViewModel { get; private set; }
        public static MainWindow Instance { get; private set; }
        public MainWindow() {       
            InitializeComponent();
            new ColorHelper().Load();
            ThemeUtils.Init();
            JsonUtils.CraftLauncherInfoJson();

            WindowWidth = Width;
            WindowHeight = Height;
            Closed += (_, x) => {
                JsonUtils.WriteLaunchInfoJson();
                JsonUtils.WriteLauncherInfoJson();
            };
            
            PointerMoved += (_, x) => {
                if (CanParallax) { 
                    Point position = x.GetPosition(BackgroundImage);
                    int xOffset = 50, yOffset = 50;
                    double num = BackgroundImage.DesiredSize.Height - position.X / (double)xOffset - BackgroundImage.DesiredSize.Height;
                    double num2 = BackgroundImage.DesiredSize.Width - position.Y / (double)yOffset - BackgroundImage.DesiredSize.Width;
                    if (!(BackgroundImage.RenderTransform is TranslateTransform)) {               
                        BackgroundImage.RenderTransform = new TranslateTransform(num, num2);
                        return;
                    }
                    TranslateTransform translateTransform = (TranslateTransform)BackgroundImage.RenderTransform;
                    if (xOffset > 0) {               
                        translateTransform.X = num;
                    }
                    
                    if (yOffset > 0) {               
                        translateTransform.Y = num2;
                    }                
                }
                else {
                    BackgroundImage.RenderTransform = new TranslateTransform(0, 0);
                }
            };
        }

        private void RemoveModLoaderClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
            var modloader = (sender as Button)!.DataContext as ModLoaderViewData;
            ViewModel.CurrentModLoaders.Remove(modloader);
        }

        public void ShowTopBar() {
            var draggableElement = topbar as IVisual;
            var transform = draggableElement!.RenderTransform as TranslateTransform ?? new();

            TranslateYAnimation animation = new(transform!.Y, WindowHeight - 125);
            animation.RunAnimation(topbar);
            IsOpen = true;
            CenterContent.Height = WindowHeight - 125;
        }

        private void MainWindow_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == HeightProperty)
            {
                if (ViewModel.CurrentPage is HomePage)
                {
                    var page = ViewModel.CurrentPage as HomePage;
                    if (HomePage.ViewModel.Isopen) {
                        HomePage.ViewModel.PanelHeight = Height - 160;
                    }
                }

                WindowHeight = e.NewValue!.ToDouble();
                var transform = topbar.RenderTransform as TranslateTransform;
                if (transform == null)
                {
                    transform = new TranslateTransform();
                    topbar.RenderTransform = transform;
                }

                if (IsOpen)
                {
                    transform!.Y = WindowHeight - 125;
                    Y = WindowHeight - 125;
                }
                else
                {
                    transform!.Y = 0;
                    Y = 0;
                }

                CenterContent.Height = transform!.Y;
            }
            else if (e.Property == WidthProperty)
            {
                WindowWidth = e.NewValue!.ToDouble();
            }
        }

        private async void WindowsInitialized(object? sender, EventArgs e)
        {
            this.Hide();
            await Task.Delay(800);
            DataContext = ViewModel = new();

            foreach (Button i in Installer.Children) {
                i.Click += (x, _) => {
                    var sender = x as Button;

                    ViewModel.ChangeTitle($"选择一个 {sender.Tag} 版本");
                    ViewModel.CurrentModLoader = null;
                    ViewModel.McVersionVisible = false;
                    ViewModel.ModLoaderVisible = true;

                    ViewModel.ModLoaders = sender.Tag switch { 
                        "Forge" => ViewModel.forges.Select(x =>
                        {
                            var data = x.CreateViewData<ModLoaderModel, ModLoaderViewData>();
                            data.Type = $"标准版本 {data.Data.Time.ToString(@"yyyy\-MM\-dd hh\:mm")}";

                            return data;
                        }).ToObservableCollection(),
                        "Fabric" => ViewModel.fabrics.Select(x =>
                        {
                            var data = x.CreateViewData<ModLoaderModel, ModLoaderViewData>();
                            data.Type = $"标准版本 {data.Data.Time.ToString(@"yyyy\-MM\-dd hh\:mm")}";

                            return data;
                        }).ToObservableCollection(),
                        "Optifine" => ViewModel.optifine.Select(x =>
                        {
                            var data = x.CreateViewData<ModLoaderModel, ModLoaderViewData>();
                            data.Type = $"标准版本 {data.Data.Time.ToString(@"yyyy\-MM\-dd hh\:mm")}";

                            return data;
                        }).ToObservableCollection(),
                        "Quilt" => ViewModel.quilt.Select(x =>
                        {
                            var data = x.CreateViewData<ModLoaderModel, ModLoaderViewData>();
                            data.Type = $"标准版本 {data.Data.Time.ToString(@"yyyy\-MM\-dd hh\:mm")}";

                            return data;
                        }).ToObservableCollection(),
                        _ => new()
                    };
                };
                
                if (i.Tag as string is "Optifine") {
                    i.PointerEnter += (_, _) => {
                        i.Margin = new(0, 0, -75, 0);
                    };

                    i.PointerLeave += (_, _) => {
                        i.Margin = new(0);
                    };
                    
                    continue;
                }

                i.PointerEnter += (_, _) => {
                    i.Margin = new(0, 0, -60, 0);
                };

                i.PointerLeave += (_, _) => {
                    i.Margin = new(0);
                };
            }

            InstallDialog.HideDialog();            
            ToolBar.InitStartAnimation();
            ToolBar.HostWindows = Instance = this;
            PropertyChanged += MainWindow_PropertyChanged;
            Drop.PointerPressed += Drop_PointerPressed;

            topbar.PointerMoved += Topbar_PointerMoved;
            topbar.PointerPressed += Topbar_PointerPressed;
            topbar.PointerReleased += Topbar_PointerReleased;

            OpenBar.PointerMoved += OpenBar_PointerMoved;
            OpenBar.PointerPressed += OpenBar_PointerPressed;
            OpenBar.PointerReleased += OpenBar_PointerReleased;

            BackgroundImage.IsVisible = App.LauncherData.BakgroundType is "图片背景";
            if (BackgroundImage.IsVisible) {           
                BackgroundImage.Source = new Bitmap(App.LauncherData.ImagePath);
            }

            ThemeUtils.SetAccentColor(App.LauncherData.AccentColor);
            CanParallax = App.LauncherData.ParallaxType is not "无";


            UpdateInfo res = await UpdateUtils.GetLatestUpdateInfoAsync();
            if (res is not null && res.CanUpdate() && SystemUtils.IsWindows)
            {
                UpdateDialog.ButtonClick += (_, _) => {
                    UpdateUtils.UpdateAsync(res, x => {
                        ViewModel.DownloadProgress = x.ToDouble() * 100;
                    }, () => {
                        int currentPID = Process.GetCurrentProcess().Id;
                        string name = Process.GetCurrentProcess().ProcessName;
                        string filename = $"{name}.exe";

                        string psCommand = $"Stop-Process -Id {currentPID} -Force;" +                                  
                                    $"Wait-Process -Id {currentPID} -ErrorAction SilentlyContinue;" +
                                    "Start-Sleep -Milliseconds 500;" +
                                    $"Remove-Item {filename} -Force;" +
                                    $"Rename-Item WonderLab.update {filename};" +
                                    $"Start-Process {name}.exe -Args updated;";

                        try
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = "powershell.exe",
                                Arguments = psCommand,
                                WorkingDirectory = Directory.GetCurrentDirectory(),
                                UseShellExecute = true,
                                WindowStyle = ProcessWindowStyle.Hidden,
                            });
                        }
                        catch (Exception ex)
                        { }
                    });
                };

                BodyMessage.Text = res.Message;
                UpdateDialog.Title = $"有新的版本推送，版本编号 {res.TagName}";
                EndMessage.Text = $"于 {res.CreatedAt} 由 Xilu 修改并推送";
                await Task.Delay(1000);
                UpdateDialog.ShowDialog();
            }

            JsonUtils.CraftLaunchInfoJson();
        }

        private void Drop_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            BeginMoveDrag(e);
        }

        public void ShowInfoDialog(string title, string message)
        {
            MainDialog.Title = title;
            MainDialog.Message = message;
            MainDialog.ShowDialog();
        }

        public void ShowInfoBar(string title, string message, HideOfRunAction action)
        {
            MessageTipsBar bar = new MessageTipsBar(action)
            {
                Title = title,
                Message = message,
            };

            grid.Children.Add(bar);
            bar.Opened += async (_, _) =>
            {
                await Task.Delay(3000);
                bar.HideDialog();
            };

            bar.ShowDialog();
        }

        public void ShowInfoBar(string title, string message)
        {
            MessageTipsBar bar = new MessageTipsBar()
            {
                Title = title,
                Message = message,
            };
            bar.HideOfRun = new(() =>
            {
                grid.Children.Remove(bar);
            });

            grid.Children.Add(bar);
            bar.Opened += async (_, _) =>
            {
                await Task.Delay(3000);
                bar.HideDialog();
            };

            bar.ShowDialog();
        }

        public async void NavigationPage(UserControl control)
        {
            try
            {
                Page.Opacity = 0;
                await Task.Delay(200);
                Page.Content = control;
                await Task.Delay(150);
                Page.Opacity = 1;
            }
            catch (Exception)
            { }           
        }

        public async void OutBar()
        {
            TranslateXAnimation animation = new(0, 100);
            animation.RunAnimation(ToolBar);

            await Task.Delay(20);

            if (ViewModel.CurrentPage is HomePage)
            {
                TranslateYAnimation animation1 = new(0, 100);
                animation1.RunAnimation((ViewModel.CurrentPage as HomePage).bab);
            }
        }

        #region 拖动组件事件

        private void Topbar_PointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
        {
            isDragging = false;
            var draggableElement = sender as IVisual;
            var transform = draggableElement!.RenderTransform as TranslateTransform;

            if (transform!.Y != 0 && transform.Y <= WindowHeight / 2)
            {
                TranslateYAnimation animation = new(transform.Y, 0);
                animation.RunAnimation(topbar);
                IsOpen = false;
                CenterContent.Height = 0;
            }
            else
            {
                TranslateYAnimation animation = new(transform.Y, WindowHeight - 125);
                animation.RunAnimation(topbar);
                IsOpen = true;
                CenterContent.Height = WindowHeight - 125;
            }
        }

        private void Topbar_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            isDragging = true;
            var draggableElement = sender as IVisual;
            var clickPosition = e.GetPosition(this);

            var transform = draggableElement.RenderTransform as TranslateTransform;
            if (transform == null)
            {
                transform = new TranslateTransform();
                draggableElement.RenderTransform = transform;
            }

            Y = clickPosition.Y - transform.Y;
        }

        private void Topbar_PointerMoved(object? sender, Avalonia.Input.PointerEventArgs e)
        {
            var draggableElement = sender as IVisual;
            if (isDragging && draggableElement != null)
            {
                Point currentPosition = e.GetPosition(this.Parent);
                var transform = draggableElement.RenderTransform as TranslateTransform;
                if (transform == null)
                {
                    transform = new TranslateTransform();
                    draggableElement.RenderTransform = transform;
                }
                if (transform.Y <= WindowHeight)
                {
                    transform.Y = currentPosition.Y - Y;
                    CenterContent.Height = transform.Y;
                }
            }
        }

        private void OpenBar_PointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
        {
            IsUseDragging = false;
            var draggableElement = sender as IVisual;
            var transform = draggableElement!.RenderTransform as TranslateTransform;

            if (transform!.X != 0 && transform.X <= WindowWidth / 4)
            {
                TranslateXAnimation animation = new(transform.X, 0);
                animation.RunAnimation(OpenBar);
                OpacityChangeAnimation opacity = new(true) {
                    RunValue = Back.Opacity
                };
                opacity.RunAnimation(Back);
            }
            else
            {
                TranslateXAnimation animation = new(transform.X, WindowWidth);
                animation.RunAnimation(OpenBar);
                OutBar();
                OpacityChangeAnimation opacity = new(false) {               
                    RunValue = Back.Opacity
                };
                opacity.AnimationCompleted += (_, _) => { OpenBar.IsVisible = false; OpenBar.IsHitTestVisible = false; };
                opacity.RunAnimation(Back);
                NavigationPage(new ActionCenterPage());                                
            }

        }

        private void OpenBar_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            IsUseDragging = true;

            var draggableElement = sender as IVisual;
            var clickPosition = e.GetPosition(this);

            var transform = draggableElement.RenderTransform as TranslateTransform;
            if (transform == null)
            {
                transform = new TranslateTransform();
                draggableElement.RenderTransform = transform;
            }

            X = clickPosition.X - transform.X;
        }

        private void OpenBar_PointerMoved(object? sender, Avalonia.Input.PointerEventArgs e)
        {
            var draggableElement = sender as IVisual;
            if (IsUseDragging && draggableElement != null)
            {
                Point currentPosition = e.GetPosition(this.Parent);
                var transform = draggableElement.RenderTransform as TranslateTransform;
                if (transform == null)
                {
                    transform = new TranslateTransform();
                    draggableElement.RenderTransform = transform;
                }
                if (transform.X <= WindowWidth)
                {
                    transform.X = currentPosition.X - X;
                    Back.Opacity = transform.X / WindowWidth;
                    //CenterContent.Width = transform.X;
                }
            }
        }

        #endregion
    }
}

