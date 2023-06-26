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
    public class TransformXAnimation : IAnimation
    {
        public bool IsReversed { get; set; }

        public event EventHandler<EventArgs>? AnimationCompleted;

        public TransformXAnimation(bool reversed) {
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
                            new Setter(ScaleTransform.ScaleXProperty, IsReversed ? 1.0 : 0.0),
                        },
                        Cue = new Avalonia.Animation.Cue(0d)
                    },
                    new Avalonia.Animation.KeyFrame
                    {
                        Setters =
                        {
                            new Setter(ScaleTransform.ScaleXProperty, IsReversed ? 0 : 1.0),
                        },
                        Cue = new Avalonia.Animation.Cue(1d)
                    }
                },
                Duration = TimeSpan.FromSeconds(IsReversed ? 0.35 : 0.75),
                FillMode = Avalonia.Animation.FillMode.Forward
            };

            await animation.RunAsync(ctrl, null);
            (ctrl as IVisual)!.Opacity = 1;

            //AnimationCompleted!.Invoke(this, new());
        }
    }
}
