using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Chrome;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using wonderlab.control.Animation;

namespace wonderlab.control.Controls.Bar
{
    public class ToolBar : TemplatedControl
    {
        //Border Border = null;
        StackPanel StackPanel = null;

        public Window HostWindows;

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {       
            base.OnApplyTemplate(e);
            //Border = e.NameScope.Find<Border>("MainBorder");
            //StackPanel = e.NameScope.Find<StackPanel>("ButtonGrounp");
            e.NameScope.Find<Button>("Close").Click += ToolBar_Click;
            e.NameScope.Find<Button>("Mini").Click += ToolBar_Click1;
        }

        public void ToolBar_Click1(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
            Mini();
        }

        public void ToolBar_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {       
            Close();
        }

        public async void InitStartAnimation() {            
            //Border.Height = 180;
            await Task.Delay(500);
            //StackPanel.Opacity = 1;
        }

        public async void CloseBarAnimation() {
            //StackPanel.Opacity = 0;
            await Task.Delay(300);
            //Border.Height = 0;
        }

        public void Mini() {
            HostWindows.WindowState = WindowState.Minimized; 
        }

        public void Close() {        
            HostWindows.Close();
        }
    }
}
