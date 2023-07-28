using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.control.Interface;

namespace wonderlab.control.Animation
{
    /// <summary>
    /// 可视度变化动画
    /// </summary>
    public class OpacityChangeAnimation : Interface.IAnimation
    {
        /// <summary>
        /// 反转动画
        /// </summary>
        public bool IsReversed { get; set; }

        public double RunValue { get; set; } = 1d;

        public OpacityChangeAnimation(bool isReversed) {
            IsReversed = isReversed;
        }

        public event EventHandler<EventArgs> AnimationCompleted;

        public async void RunAnimation(Animatable ctrl) {      
            var animation = new Avalonia.Animation.Animation
            {
                Easing = new SplineEasing(0.1, 0.9, 0.2, 1.0),
                Children =
                {
                    new KeyFrame
                    {
                        Setters = {                       
                            new Setter(Visual.OpacityProperty, RunValue),
                        },
                        Cue = new Cue(0d)
                    },
                    new KeyFrame
                    {
                        Setters = {                       
                            new Setter(Visual.OpacityProperty, IsReversed ? 0.0d : 1d),
                        },
                        Cue = new Cue(1d)
                    }
                },
                Duration = TimeSpan.FromSeconds(0.67),
                FillMode = FillMode.Forward
            };

            await animation.RunAsync(ctrl);

            (ctrl as Visual)!.Opacity = IsReversed ? 0.0d : 1d;  
            AnimationCompleted?.Invoke(this, new EventArgs());
        }
    }
}
