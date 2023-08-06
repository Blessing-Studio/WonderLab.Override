using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.control.Animation;

namespace wonderlab.control.Controls {
    [Obsolete]
    public class Parallax3dImage : Image {
        public Parallax3dImage() {
            base.RenderTransform = new TransformGroup {
                Children = {               
                    new Rotate3DTransform(),
                }
            };

            Stretch = Stretch.UniformToFill;
            base.PointerMoved += OnPointerMoved;
            //base.PointerExited += OnPointerExited;
        }

        private Rotate3DTransform GetRotate3DTransform() {
            return (Rotate3DTransform)((TransformGroup)base.RenderTransform!).Children.First();
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e) {
            double num = (e.GetPosition(this).X / base.Bounds.Width - 0.5) * 5;
            double num2 = (0.0 - (e.GetPosition(this).Y / base.Bounds.Height - 0.5)) * 2;
            Rotate3DTransform rotate3DTransform = GetRotate3DTransform();
            rotate3DTransform.Depth = 300.0;
            rotate3DTransform.CenterX = num;
            rotate3DTransform.CenterY = num2;
            rotate3DTransform.AngleX = num;
            rotate3DTransform.AngleY = num2;
        }

        private void OnPointerExited(object? sender, PointerEventArgs e) {
            var rotate3DTransform = GetRotate3DTransform();
            rotate3DTransform.Depth = 0.0;
            rotate3DTransform.CenterX = base.Bounds.Center.X;
            rotate3DTransform.CenterY = base.Bounds.Center.Y;
            rotate3DTransform.AngleX = 0.0;
            rotate3DTransform.AngleY = 0.0;
        }
    }
}
