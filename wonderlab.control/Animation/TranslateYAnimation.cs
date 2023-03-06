using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.control.Interface;

namespace wonderlab.control.Animation
{
    public class TranslateYAnimation : IAnimation
    {
        [Obsolete]
        public bool IsReversed { get; set; }

        public double StartPoint { get; set; }

        public double EndPoint { get; set; }

        public event EventHandler<EventArgs>? AnimationCompleted;

        public TranslateYAnimation(double s, double e) {       
            StartPoint= s;
            EndPoint= e;
        }

        public async void RunAnimation(Avalonia.Animation.Animatable ctrl)
        {
            var animation = new Avalonia.Animation.Animation
            {
                Easing = new Avalonia.Animation.Easings.SplineEasing(0.1, 0.9, 0.2, 1.0),
                Children =
                {
                    new Avalonia.Animation.KeyFrame
                    {
                        Setters =
                        {
                            //new Setter(Visual.OpacityProperty, 0.0),
                            new Setter(TranslateTransform.YProperty, StartPoint),
                        },
                        Cue = new Avalonia.Animation.Cue(0d)
                    },
                    new Avalonia.Animation.KeyFrame
                    {
                        Setters =
                        {
                            //new Setter(Visual.OpacityProperty, 1.0),
                            new Setter(TranslateTransform.YProperty, EndPoint),
                        },
                        Cue = new Avalonia.Animation.Cue(1d)
                    }
                },
                Duration = TimeSpan.FromSeconds(0.67),
                FillMode = Avalonia.Animation.FillMode.Forward
            };

            await animation.RunAsync(ctrl, null);
            (ctrl as IVisual)!.Opacity = 1;

            AnimationCompleted?.Invoke(this, new());
        }
    }
}
