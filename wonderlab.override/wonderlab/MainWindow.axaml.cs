using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Microsoft.VisualBasic;
using MinecaftOAuth.Authenticator;
using MinecraftLaunch.Modules.Installer;
using MinecraftLaunch.Modules.Models.Auth;
using MinecraftLaunch.Modules.Toolkits;
using Newtonsoft.Json;
using SkiaSharp;
using System;
using System.Collections.Generic;
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
            JsonUtils.CraftLaunchInfoJson();
            InitializeComponent();
            new ColorHelper().Load();
            ThemeUtils.Init();
            JsonUtils.CraftLauncherInfoJson();
            Instance = this;

            this.AddHandler(DragDrop.DropEvent, DropAction);
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

            close.Click += (_, _) => Close();
            Mini.Click += (_, _) => WindowState = WindowState.Minimized;
            NotificationCenterButton.Click += (_, _) => NotificationCenter.Open();
        }

        public async void DropAction(object? sender, DragEventArgs e) {
            if (e.Data.Contains(DataFormats.FileNames)) {
                var result = e.Data.GetFileNames();
                if (result!.Count() > 1) {
                    "一次只能拖入一个文件".ShowMessage("提示");
                    return;
                }

                "开始分析文件类型，此过程会持续两到三秒，请耐心等待".ShowMessage("提示");
                var file = result!.FirstOrDefault()!;
                if (!file.IsFile() || !(file.EndsWith(".zip") || file.EndsWith(".mrpack"))) {
                    "WonderLab 未能确定此文件格式应当执行的相关操作".ShowMessage("错误");
                    return;
                }

                await Task.Delay(1000);
                await ModpacksUtils.ModpacksInstallAsync(file);
            }
        }

        private void MainWindow_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == HeightProperty)
            {
                if (ViewModel.CurrentPage is HomePage)
                {
                    var page = ViewModel.CurrentPage as HomePage;
                    if (HomePage.ViewModel.Isopen) {
                        HomePage.ViewModel.PanelHeight = Height - 180;
                    }
                }

                WindowHeight = e.NewValue!.ToDouble();
            }
            else if (e.Property == WidthProperty)
            {
                WindowWidth = e.NewValue!.ToDouble();
            }
        }

        private async void WindowsInitialized(object? sender, EventArgs e)
        {
            await Task.Delay(500);
            DataContext = ViewModel = new();

            BackgroundImage.IsVisible = App.LauncherData.BakgroundType is "图片背景";
            ThemeUtils.SetAccentColor(App.LauncherData.AccentColor);
            CanParallax = App.LauncherData.ParallaxType is not "无";

            PropertyChanged += MainWindow_PropertyChanged;
            Drop.PointerPressed += Drop_PointerPressed;

            OpenBar.PointerMoved += OpenBar_PointerMoved;
            OpenBar.PointerPressed += OpenBar_PointerPressed;
            OpenBar.PointerReleased += OpenBar_PointerReleased;

            if (BackgroundImage.IsVisible && !string.IsNullOrEmpty(App.LauncherData.ImagePath)) {
                BackgroundImage.Source = new Bitmap(App.LauncherData.ImagePath);
            }

            UpdateInfo res = await UpdateUtils.GetLatestUpdateInfoAsync();
            if (res is not null && res.CanUpdate() && SystemUtils.IsWindows)
            {
                UpdateDialog.AcceptButtonClick += (_, _) => {
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

                            var intVersion = Convert.ToInt32(res.TagName.Replace(".", string.Empty));
                            App.LauncherData.LauncherVersion = intVersion;
                            JsonUtils.WriteLauncherInfoJson();
                        }
                        catch (Exception)
                        { }
                    });
                };
                UpdateDialog.CloseButtonClick += (_, _) => { UpdateDialog.HideDialog(); };

                BodyMessage.Text = res.Message;
                UpdateDialog.Title = $"有新的版本推送，版本编号 {res.TagName}";
                EndMessage.Text = $"于 {res.CreatedAt} 由 Xilu 修改并推送";
                await Task.Delay(1000);
                UpdateDialog.ShowDialog();
            }

            StreamReader reader = new(AvaloniaUtils.GetAssetsStream("ModpackInfos.json"));
            var infos = reader.ReadToEnd().ToJsonEntity<List<WebModpackInfoModel>>();
            if(infos is not null && infos.Any()) {
                infos.ForEach(x => {
                    if (x.CurseForgeId != null) {                   
                        DataUtil.WebModpackInfoDatas.Add(x.CurseForgeId, x);
                    }
                });

                DataUtil.WebModpackInfoDatas.Values.ToList().ForEach(x => {               
                    if (x.Chinese.Contains("*"))
                        x.Chinese = x.Chinese.Replace("*", string.Empty);                           
                });

                Trace.WriteLine(DataUtil.WebModpackInfoDatas.Count);
            }
        }

        private void Drop_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            try {           
                BeginMoveDrag(e);
            }
            catch (Exception) {           
            }
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
            Dispatcher.UIThread.Post(() =>
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
            });
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

        public async void CloseTopBar() {
            TopBar1.Margin = new(0, -150, 0, 0);
            await Task.Delay(50);
            TopBar2.Margin = new(0, -100, 0, 0);
            await Task.Delay(50);
            TopBar3.Margin = new(0, -100, 0, 0);
        }

        public async void OpenTopBar() {       
            TopBar3.Margin = new(0);
            await Task.Delay(50);
            TopBar2.Margin = new(0);
            await Task.Delay(50);
            TopBar1.Margin = new(0);
        }

        #region 拖动组件事件

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
                OpacityChangeAnimation opacity = new(false) {               
                    RunValue = Back.Opacity
                };
                opacity.AnimationCompleted += (_, _) => { OpenBar.IsVisible = false; OpenBar.IsHitTestVisible = false; };
                opacity.RunAnimation(Back);
                NavigationPage(new ActionCenterPage());
                CloseTopBar();
            }

        }

        private void OpenBar_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            IsUseDragging = true;

            var draggableElement = sender as IVisual;
            var clickPosition = e.GetPosition(this);

            var transform = draggableElement!.RenderTransform as TranslateTransform;
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

