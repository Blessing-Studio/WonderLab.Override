using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using MinecraftLaunch.Modules.Models.Install;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WonderLab.Classes.Medias {
    public class PageTransition : IPageTransition {
        public PageTransition(TimeSpan duration) {
            Duration = duration;
        }

        public bool Fade { get; set; }

        public bool IsHorizontal { get; set; } = false;

        public TimeSpan Duration { get; set; }

        public Easing Easing { get; set; } = new CircularEaseInOut();

        public virtual async Task Start(Visual? from, Visual? to, bool forward, CancellationToken cancellationToken) {
            if (cancellationToken.IsCancellationRequested) {
                return;
            }

            var tasks = new List<Task>();
            if (from != null) {
                var animation = new Animation {
                    Easing = this.Easing,
                    FillMode = FillMode.Forward,
                    Children = {
                       new KeyFrame {
                           Setters = {
                               new Setter {
                                   Property = ScaleTransform.ScaleXProperty,
                                   Value = 1d
                               },
                               new Setter {
                                   Property = ScaleTransform.ScaleYProperty,
                                   Value = 1d
                               },
                               new Setter {
                                   Property = Visual.OpacityProperty,
                                   Value = 1d
                               }
                           },

                           Cue = new Cue(0d)
                       },
                       new KeyFrame {
                          Setters = {
                              new Setter {
                                  Property = Visual.OpacityProperty,
                                  Value = 0d
                              },
                              new Setter {
                                  Property = ScaleTransform.ScaleXProperty,
                                  Value = 0.85d
                              },
                              new Setter {
                                  Property = ScaleTransform.ScaleYProperty,
                                  Value = 0.85d
                              },
                          },
                           
                          Cue = new Cue(1d)
                       }
                    },

                    Duration = Duration
                };

                tasks.Add(animation.RunAsync(from, 
                    cancellationToken));
            }

            if (to != null) {
                to.IsVisible = true;
                var animation = new Animation {
                    Easing = this.Easing,
                    FillMode = FillMode.Forward,
                    Children = {
                       new KeyFrame {
                           Setters = {
                               new Setter {
                                   Property = ScaleTransform.ScaleXProperty,
                                   Value = 1d
                               },
                               new Setter {
                                   Property = ScaleTransform.ScaleYProperty,
                                   Value = 1d
                               },
                               new Setter {
                                   Property = Visual.OpacityProperty,
                                   Value = 1d
                               }
                           },

                           Cue = new Cue(0d)
                       },
                       new KeyFrame {
                          Setters = {
                              new Setter {
                                  Property = Visual.OpacityProperty,
                                  Value = 0d
                              },
                              new Setter {
                                  Property = ScaleTransform.ScaleXProperty,
                                  Value = 0.85d
                              },
                              new Setter {
                                  Property = ScaleTransform.ScaleYProperty,
                                  Value = 0.85d
                              },
                          },

                          Cue = new Cue(1d)
                       }
                    },

                    Duration = Duration
                };

                tasks.Add(animation.RunAsync(to,
                    cancellationToken));
            }


            await Task.WhenAll(tasks);
            if (from != null && !cancellationToken.IsCancellationRequested) {
                from.IsVisible = false;
            }
        }
    }
}