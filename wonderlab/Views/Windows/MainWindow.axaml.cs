using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using MinecraftLaunch.Modules.Models.Download;
using MinecraftLaunch.Modules.Toolkits;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using wonderlab.Class.AppData;
using wonderlab.Class.Models;
using wonderlab.Class.Utils;
using wonderlab.control.Controls.Bar;
using wonderlab.control.Controls.Dialog;
using wonderlab.ViewModels.Windows;
using wonderlab.Views.Pages;
using static wonderlab.control.Controls.Bar.MessageTipsBar;

namespace wonderlab.Views.Windows {
    public partial class MainWindow : Window {
        public double WindowHeight, WindowWidth;

        public bool IsOpen, CanParallax;

        public static MainWindowViewModel ViewModel { get; private set; }

        public MainWindow() {
            JsonUtils.CreateLaunchInfoJson();
            Initialized += DataInitialized;

            InitializeComponent();

            try {
                ThemeUtils.Init();
                JsonUtils.CreateLauncherInfoJson();

                DataContext = ViewModel = new();
                AddHandler(DragDrop.DropEvent, DropAction);
                WindowWidth = Width;
                WindowHeight = Height;

                Closed += (_, x) => {
                    JsonUtils.WriteLaunchInfoJson();
                    JsonUtils.WriteLauncherInfoJson();
                };

                PointerMoved += OnPointerMoved;
                PropertyChanged += OnPropertyChanged;

                close.Click += (_, _) => Close();
                Mini.Click += (_, _) => WindowState = WindowState.Minimized;
                NotificationCenterButton.Click += (_, _) => NotificationCenter.Open();
            }
            catch (Exception ex) {
                $"初始化信息失败，错误信息如下：{ex.Message}".ShowMessage("Error");
            }
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e) {
            if (CanParallax) {
                Point position = e.GetPosition(BackgroundImage);
                int xOffset = 50, yOffset = 50;

                double num = BackgroundImage.DesiredSize.Height - position.X / xOffset - BackgroundImage.DesiredSize.Height;
                double num2 = BackgroundImage.DesiredSize.Width - position.Y / yOffset - BackgroundImage.DesiredSize.Width;

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
            } else {
                BackgroundImage.RenderTransform = new TranslateTransform(0, 0);
            }
        }

        private async void DataInitialized(object? sender, EventArgs e) {
            await Task.Delay(500);

            try {
                if (App.LauncherData.CurrentDownloadAPI == APIManager.Mcbbs) {
                    APIManager.Current = APIManager.Mcbbs;
                } else if (App.LauncherData.CurrentDownloadAPI == APIManager.Bmcl) {
                    APIManager.Current = APIManager.Bmcl;
                } else if (App.LauncherData.CurrentDownloadAPI == APIManager.Mojang) {
                    APIManager.Current = APIManager.Mojang;
                } else {
                    App.LauncherData.CurrentDownloadAPI = APIManager.Current;
                }

                BackgroundImage.IsVisible = App.LauncherData.BakgroundType is "图片背景";
                ThemeUtils.SetAccentColor(App.LauncherData.AccentColor);
                CanParallax = App.LauncherData.ParallaxType is not "无";

                if (BackgroundImage.IsVisible && !string.IsNullOrEmpty(App.LauncherData.ImagePath)) {
                    BackgroundImage.Source = new Bitmap(App.LauncherData.ImagePath);
                }
            }
            catch {
                if (App.LauncherData.IsNull()) {
                    App.LauncherData = new();
                }
            }
        }

        private void OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e) {
            if (e.Property == HeightProperty) {
                if (ViewModel.CurrentPage is HomePage) {
                    if (HomePage.ViewModel.Isopen) {
                        HomePage.ViewModel.PanelHeight = Height - 180;
                    }
                }

                WindowHeight = e.NewValue!.ToDouble();
            } else if (e.Property == WidthProperty) {
                WindowWidth = e.NewValue!.ToDouble();
            }
        }

        private async void WindowsInitialized(object? sender, EventArgs e) {
            await Task.Delay(500);

            try {
                Drop.PointerPressed += OnPointerPressed;

                UpdateInfo result = await UpdateUtils.GetLatestUpdateInfoAsync();
                $"开始自动更新流程，当前启动器版本序列号 {App.LauncherData.LauncherVersion}".ShowLog();

                if (result is not null && result.CanUpdate() && SystemUtils.IsWindows) {
                    UpdateDialog.CloseButtonClick += (_, _) => {
                        UpdateUtils.UpdateAsync(result, x => {
                            ViewModel.DownloadProgress = x.ToDouble() * 100;
                        });
                    };

                    BodyMessage.Text = result.Message;
                    UpdateDialog.Title = $"有新的版本推送，版本编号 {result.TagName}";
                    EndMessage.Text = $"于 {result.CreatedAt} 由 Xilu 修改并推送";
                    await Task.Delay(1000);
                    UpdateDialog.ShowDialog();
                }
            }
            catch (Exception ex) {
                $"{ex.Message}".ShowMessage();
            }

            CacheResources.GetWebModpackInfoData();
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e) {
            try {
                BeginMoveDrag(e);
            }
            catch (Exception) {
                "错误，在拖动窗口时遇到了点小麻烦".ShowMessage();
            }
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

        public void ShowInfoDialog(string title, string message) {
            MainDialog.Title = title;
            MainDialog.Message = message;
            MainDialog.ShowDialog();
        }

        public void ShowInfoBar(string title, string message, HideOfRunAction action) {
            MessageTipsBar bar = new MessageTipsBar(action) {
                Title = title,
                Message = message,
            };

            grid.Children.Add(bar);
            bar.Opened += async (_, _) => {
                await Task.Delay(3000);
                bar.HideDialog();
            };

            bar.ShowDialog();
        }

        public void ShowInfoBar(string title, string message) {
            Dispatcher.UIThread.Post(() => {
                MessageTipsBar bar = new MessageTipsBar() {
                    Title = title,
                    Message = message,
                };
                bar.HideOfRun = new(() => {
                    grid.Children.Remove(bar);
                });

                grid.Children.Add(bar);
                bar.Opened += async (_, _) => {
                    await Task.Delay(3000);
                    bar.HideDialog();
                };

                bar.ShowDialog();
            });
        }

        public async void Navigation(UserControl control) {
            try {
                Page.Opacity = 0;
                await Task.Delay(200);
                Page.Content = control;
                await Task.Delay(150);
                Page.Opacity = 1;
            }
            catch (Exception) { }
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
    }

}

