using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.VisualTree;
using Microsoft.VisualBasic;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using wonderlab.Class.Utils;
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
        private bool isDragging, IsOpen, IsUseDragging;
        public static MainWindowViewModel ViewModel { get; private set; }
        public static MainWindow Instance { get; private set; }
        public MainWindow()
        {
            InitializeComponent();
            new ColorHelper().Load();
            WindowWidth = Width;
            WindowHeight = Height;
        }

        private void MainWindow_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == HeightProperty)
            {
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

        private async void WindowsInitialized(object? sender, EventArgs e)
        {
            await Task.Delay(800);
            DataContext = ViewModel = new();
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

            UpdateInfo res = new();//await UpdateUtils.GetLatestUpdateInfoAsync();
            if (res is not null && res.CanUpdate())
            {
                UpdateDialog.ButtonClick += (_, _) => {
                    UpdateUtils.UpdateAsync(res, x => {
                        ViewModel.DownloadProgress = x.ToDouble() * 100;
                    }, () => {
                        int currentPID = Process.GetCurrentProcess().Id;
                        string name = Process.GetCurrentProcess().ProcessName;
                        string filename = $"{name}.exe";

                        string psCommand =
                                    $"Stop-Process -Id {currentPID} -Force;" +
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

                UpdateDialog.Message = $"�汾��� {res.TagName}\n�� {res.CreatedAt.ToString(@"yyyy\-MM\-dd hh\:mm")} �� {res.Author.Name} �޸Ĳ�����";
                await Task.Delay(1000);
                UpdateDialog.ShowDialog();
            }

            JsonUtils.CraftLaunchInfoJson();
        }

        private void OpenBar_PointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
        {
            IsUseDragging = false;
            var draggableElement = sender as IVisual;
            var transform = draggableElement!.RenderTransform as TranslateTransform;

            if (transform!.X != 0 && transform.X <= WindowWidth / 2)
            {
                TranslateXAnimation animation = new(transform.X, 0);
                animation.RunAnimation(OpenBar);
                OpacityChangeAnimation opacity = new(true);
                opacity.RunAnimation(Back);
            }
            else
            {
                TranslateXAnimation animation = new(transform.X, WindowWidth);
                animation.RunAnimation(OpenBar);
                OutBar();
                OpacityChangeAnimation opacity = new(false);
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
            Page.Opacity = 0;
            await Task.Delay(200);
            Page.Content = control;
            await Task.Delay(150);
            Page.Opacity = 1;
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
    }
}

//void addSubDirectory(DirectoryInfo directory, string pattern)
//{
//    try
//    {
//        foreach (FileInfo fi in directory.GetFiles(pattern))
//        {
//            addrelativeDocument(fi.FullName);
//        }

//        foreach (DirectoryInfo di in directory.GetDirectories())
//        {
//            addSubDirectory(di, pattern);
//        }
//    }
//    catch (Exception)
//    {

//    }
//}

//void addrelativeDocument(string path)
//{
//    ra++;
//}