using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.control {
    public static class ParallaxUtil {
        public static void RunFlatParallax(Control control, Point position) {
            int xOffset = 50, yOffset = 50;

            Size desiredSize = control.DesiredSize;
            double num = desiredSize.Height - position.X / xOffset - desiredSize.Height;
            double num2 = desiredSize.Width - position.Y / yOffset - desiredSize.Width;

            if (!(control.RenderTransform is TranslateTransform)) {
                control.RenderTransform = new TranslateTransform(num, num2);
                return;
            }

            TranslateTransform translateTransform = (TranslateTransform)control.RenderTransform;
            if (xOffset > 0) {
                translateTransform.X = num;
            }

            if (yOffset > 0) {
                translateTransform.Y = num2;
            }
        }
    }
}
