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
using IAnimation = wonderlab.control.Interface.IAnimation;

namespace wonderlab.control.Animation
{
    public class MessageTipsBarClickAnimation : IAnimation
    {
        public bool IsReversed { get; set; }

        public event EventHandler<EventArgs> AnimationCompleted;

        public async void RunAnimation(Animatable ctrl)
        {
            var animation = new Avalonia.Animation.Animation
            {
                Easing = new ExponentialEaseOut(),
                Children =
                {
                    new KeyFrame
                    {
                        Setters =
                        {
                            new Setter(ScaleTransform.ScaleXProperty, 1d),
                            new Setter(ScaleTransform.ScaleYProperty, 1d),
                        },
                        Cue= new Cue(0d)
                    },
                    new KeyFrame
                    {
                        Setters =
                        {
                            new Setter(ScaleTransform.ScaleXProperty, 0.80d),
                            new Setter(ScaleTransform.ScaleYProperty, 0.80d),
                        },
                        Cue= new Cue(1d)
                    },
                },
                Duration = TimeSpan.FromSeconds(0.17),
                FillMode = FillMode.Forward
            };

            await animation.RunAsync(ctrl, null);
            //await Task.Delay(200);

            var Endanimation = new Avalonia.Animation.Animation
            {
                Easing = new ExponentialEaseOut(),
                Children =
                {
                    new KeyFrame
                    {
                        Setters =
                        {
                            new Setter(Visual.OpacityProperty, 1.0d),
                        },
                        Cue= new Cue(0d)
                    },
                    new KeyFrame
                    {
                        Setters =
                        {
                            new Setter(Visual.OpacityProperty, 0.0d),
                        },
                        Cue= new Cue(1d)
                    },
                },
                Duration = TimeSpan.FromSeconds(0.27),
                FillMode = FillMode.Forward
            };

            await Endanimation.RunAsync(ctrl, null);

            (ctrl as IVisual).IsVisible = false;

            AnimationCompleted?.Invoke(this, new());
        }
    }
}
