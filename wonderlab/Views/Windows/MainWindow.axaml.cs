using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using DialogHostAvalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wonderlab.Class.AppData;
using wonderlab.Class.Enum;
using wonderlab.Class.Utils;
using wonderlab.control;
using wonderlab.ViewModels.Windows;
using wonderlab.Views.Dialogs;
using wonderlab.Views.Pages;
using static wonderlab.control.Controls.Bar.MessageTipsBar;

namespace wonderlab.Views.Windows {
    public partial class MainWindow : Window {
        public bool CanParallax;

        public static MainWindowViewModel ViewModel { get; set; }

        public MainWindow() {
            InitializeComponent();
            AddHandler(DragDrop.DropEvent, DropAction);
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e) {
            try {
                PointerMoved += OnPointerMoved;
                PropertyChanged += OnPropertyChanged;
                Drop.PointerPressed += OnPointerPressed;
                TopInfoBar.PointerPressed += OnPointerPressed;
                TopInfoBar1.PointerPressed += OnPointerPressed;
                TopInfoBar2.PointerPressed += OnPointerPressed;

                close.Click += (_, _) => Close();
                Mini.Click += (_, _) => WindowState = WindowState.Minimized;

                NotificationCenterButton.Click += (_, _) => NotificationCenter.Open();
                CanParallax = GlobalResources.LauncherData.ParallaxType is not "无";
            }
            catch (Exception ex) {
                $"{ex.Message}".ShowMessage();
            }
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e) {
            if (CanParallax) {
                var control = BackgroundImage;
                ParallaxUtil.RunFlatParallax(control, e.GetPosition(control));
            } else {
                BackgroundImage.RenderTransform = new TranslateTransform(0, 0);
            }
        }

        private void OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e) {
            if (e.Property == HeightProperty) {
                if (ViewModel.CurrentPage is HomePage) {
                    if (HomePage.ViewModel.Isopen) {
                        HomePage.ViewModel.PanelHeight = Height - 180;
                    }
                }
            }
        }
        
        private void OnPointerPressed(object? sender, PointerPressedEventArgs e) {
            BeginMoveDrag(e);
        }

        public async void DropAction(object? sender, DragEventArgs e) {
            try {
                "检测到拖入！".ShowLog();
                var result = e.Data.GetFiles();

                if (!result.IsNull() && !result.Any()) {
                    return;
                }

                if (result!.Count() > 1) {
                    "一次只能拖入一个文件".ShowMessage("提示");
                    return;
                }

                "开始分析文件类型，此过程会持续两到三秒，请耐心等待".ShowMessage("提示");
                var file = result!.FirstOrDefault()!.ToFile();
                if (!(file.Name.EndsWith(".zip") || file.Name.EndsWith(".mrpack"))) {
                    $"WonderLab 未能确定文件 {file.Name} 的格式应当执行的相关操作".ShowMessage("错误");
                    return;
                }
                var type = ModpacksUtils.ModpacksTypeAnalysis(file.FullName);
                if (type is ModpacksType.Unknown)
                {
                    "未知整合包类型".ShowMessage();
                    return;
                }
                await ModpacksUtils.ModpacksInstallAsync(file.FullName, type);
            }
            catch (Exception) {
                "Fuck".ShowLog(LogLevel.Error);
            }
        }

        public void ShowInfoBar(string title, string message, HideOfRunAction action) {
            tipBarView.Add(title,message);
        }

        public void ShowInfoBar(string title, string message) {
            Dispatcher.UIThread.Post(() => {
                tipBarView.Add(title, message);
            }, DispatcherPriority.Background);
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


