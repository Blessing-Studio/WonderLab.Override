using Avalonia.Animation.Easings;
using Avalonia.Animation;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WonderLab.Classes.Medias {
    public class EntranceNavigationTransitionInfo : NavigationTransitionInfo {
        public double FromHorizontalOffset { get; set; } = 0;

        public double FromVerticalOffset { get; set; } = 100;

        public async override void RunAnimation(Animatable ctrl, CancellationToken cancellationToken) {
            var animation = new Animation {
                Easing = new SplineEasing(0.1, 0.9, 0.2, 1.0),
                Children =
                {
                new KeyFrame
                {
                    Setters =
                    {
                        new Setter(Visual.OpacityProperty, 0.0),
                        new Setter(TranslateTransform.XProperty,FromHorizontalOffset),
                        new Setter(TranslateTransform.YProperty, FromVerticalOffset)
                    },
                    Cue = new Cue(0d)
                },
                new KeyFrame
                {
                    Setters =
                    {
                        new Setter(Visual.OpacityProperty, 1d),
                        new Setter(TranslateTransform.XProperty,0.0),
                        new Setter(TranslateTransform.YProperty, 0.0)
                    },
                    Cue = new Cue(1d)
                }
            },
                Duration = TimeSpan.FromSeconds(0.5),
                FillMode = FillMode.Forward
            };

            await animation.RunAsync(ctrl, cancellationToken);
            (ctrl as Visual)!.Opacity = 1;
        }
    }
}
