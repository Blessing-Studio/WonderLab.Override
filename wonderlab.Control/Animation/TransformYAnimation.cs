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
    public class TransformYAnimation : IAnimation
    {
        public double RunTime { get; set; } = 0.75;

        public bool IsReversed { get; set; }

        public event EventHandler<EventArgs>? AnimationCompleted;

        public TransformYAnimation(bool reversed) {       
            IsReversed = reversed;
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
                            //new Setter(Visual.OpacityProperty, IsReversed ? 1.0 : 0.0),
                            new Setter(ScaleTransform.ScaleYProperty, IsReversed ? 1.0 : 0.0),
                        },
                        Cue = new Avalonia.Animation.Cue(0d)
                    },
                    new Avalonia.Animation.KeyFrame
                    {
                        Setters =
                        {
                            //new Setter(Visual.OpacityProperty, IsReversed ? 1.0 : 0.0),
                            new Setter(ScaleTransform.ScaleYProperty, IsReversed ? 0 : 1.0),
                        },
                        Cue = new Avalonia.Animation.Cue(1d)
                    }
                },
                Duration = TimeSpan.FromSeconds(RunTime),
                FillMode = Avalonia.Animation.FillMode.Forward
            };

            await animation.RunAsync(ctrl);
            (ctrl as Visual)!.Opacity = 1;
            (ctrl as Visual)!.RenderTransform = new ScaleTransform()
            {
                ScaleY = IsReversed ? 0 : 1.0
            };

            AnimationCompleted?.Invoke(this, new());
        }
    }
}
