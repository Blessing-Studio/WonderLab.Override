using Avalonia.Animation.Easings;
using Avalonia.Animation;
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
using IAnimation = wonderlab.control.Interface.IAnimation;

namespace wonderlab.control.Animation
{
    public class OffsetChangeAnimation : IAnimation
    {
        public double FromHorizontalOffset { get; set; } = 0;

        public double FromVerticalOffset { get; set; } = 28;

        public bool IsReversed { get; set; }
        
        public event EventHandler<EventArgs>? AnimationCompleted;

        public async void RunAnimation(Animatable ctrl) {
            var animation = new Avalonia.Animation.Animation
            {
                Easing = new ExponentialEaseOut(),
                Children =
                {
                    new KeyFrame
                    {
                        Setters =
                        {
                            //new Setter(Visual.OpacityProperty, 0.0),
                            new Setter(TranslateTransform.XProperty, IsReversed ? 0.0 : FromHorizontalOffset),
                            new Setter(TranslateTransform.YProperty, IsReversed ? 0.0 : FromVerticalOffset)
                        },
                        Cue = new Cue(0d)
                    },
                    new KeyFrame
                    {
                        Setters =
                        {
                            //new Setter(Visual.OpacityProperty, 1d),
                            new Setter(TranslateTransform.XProperty,IsReversed ? FromHorizontalOffset : 0.0),
                            new Setter(TranslateTransform.YProperty, IsReversed ? FromHorizontalOffset : 0.0)
                        },
                        Cue = new Cue(1d)
                    }
                },
                Duration = TimeSpan.FromSeconds(0.67),
                FillMode = FillMode.Forward
            };

            await animation.RunAsync(ctrl);

            AnimationCompleted?.Invoke(this, new());
            (ctrl as Visual).Opacity = 1;
        }
    }
}
