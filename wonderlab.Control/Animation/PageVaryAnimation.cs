using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace wonderlab.control.Animation {
    public class PageVaryAnimation : IPageTransition {
        public PageVaryAnimation(TimeSpan duration) {
            Duration = duration;
        }

        public bool Fade { get; set; }

        public TimeSpan Duration { get; set; }

        public Easing VaryEasing { get; set; } = new CircularEaseInOut();

        public virtual async Task Start(Visual? from, Visual? to, bool forward, CancellationToken cancellationToken) {
            if (cancellationToken.IsCancellationRequested) {
                return;
            }

            var tasks = new List<Task>();
            var parent = GetVisualParent(from, to);
            var distance = parent.Bounds.Width;
            var translateProperty = TranslateTransform.YProperty;

            if (from != null) {
                double end = forward ? -distance : distance;
                var animation = Fade ? new Avalonia.Animation.Animation {
                    Easing = VaryEasing,
                    FillMode = FillMode.Forward,
                    Children = {                   
                    new KeyFrame {                   
                        Setters = {                       
                            new Setter {                           
                                Property = translateProperty,
                                Value = 0d
                            },
                            new Setter {                           
                                Property = Visual.OpacityProperty,
                                Value = 1.0d
                            }
                        },

                        Cue = new Cue(0d)
                    },
                    new KeyFrame {                   
                        Setters = {                       
                            new Setter {                           
                                Property = Visual.OpacityProperty,
                                Value = 0d
                            }
                        },

                        Cue = new Cue(0.3d)
                    },
                    new KeyFrame {
                        Setters = {                       
                            new Setter {                           
                                Property = translateProperty,
                                Value = end
                            },
                            new Setter {                           
                                Property = Visual.OpacityProperty,
                                Value = 0d
                            }
                        },
                        Cue = new Cue(1d)
                    }
                },
                Duration = Duration

                } : new Avalonia.Animation.Animation {
                    Easing = VaryEasing,
                    FillMode = FillMode.Forward,
                    Children = {                   
                       new KeyFrame {                   
                        Setters = {                       
                            new Setter {                           
                                Property = translateProperty,
                                Value = 0d
                            },
                            new Setter {                           
                                Property = Visual.OpacityProperty,
                                Value = 1.0d
                            }
                        },
                        Cue = new Cue(0d)
                    },
                       new KeyFrame {                   
                        Setters = {                       
                            new Setter {                           
                                Property = translateProperty,
                                Value = end
                            },
                            new Setter {                           
                                Property = Visual.OpacityProperty,
                                Value = 1.0d
                            }
                        },

                        Cue = new Cue(1d)
                    }
                },
                    Duration = Duration
                };

                tasks.Add(animation.RunAsync(from, cancellationToken));
            }

            if (to != null) {
                to.IsVisible = true;
                double end = forward ? distance : -distance;
                var animation = Fade ? new Avalonia.Animation.Animation {
                    FillMode = FillMode.Forward,
                    Easing = VaryEasing,
                    Children = {                   
                    new KeyFrame {                   
                        Setters = {                       
                            new Setter {                           
                                Property = translateProperty,
                                Value = end
                            },
                            new Setter {                           
                                Property = Visual.OpacityProperty,
                                Value = 0.0d
                            }
                        },
                        Cue = new Cue(0d)
                    },
                    new KeyFrame {                   
                        Setters = {                       
                            new Setter {                           
                                Property = Visual.OpacityProperty,
                                Value = 1d
                            }
                        },
                        Cue = new Cue(0.3d)
                    },
                    new KeyFrame {                   
                        Setters = {                       
                            new Setter {                           
                                Property = translateProperty,
                                Value = 0d
                            },
                            new Setter {                           
                                Property = Visual.OpacityProperty,
                                Value = 1d
                            }
                        },
                        Cue = new Cue(1d)
                    },
                },
                    Duration = Duration
                } : new Avalonia.Animation.Animation {
                    FillMode = FillMode.Forward,
                    Easing = VaryEasing,
                    Children = {                   
                    new KeyFrame {                   
                        Setters = {                 
                            new Setter {                           
                                Property = translateProperty,
                                Value = end
                            },
                            new Setter {                           
                                Property = Visual.OpacityProperty,
                                Value = 1.0d
                            }
                        },

                        Cue = new Cue(0d)
                    },
                    new KeyFrame {                   
                        Setters = {                       
                            new Setter {                           
                                Property = translateProperty,
                                Value = 0d
                            },
                            new Setter {                           
                                Property = Visual.OpacityProperty,
                                Value = 1.0d
                            }
                        },

                        Cue = new Cue(1d)
                    }
                },
                    Duration = Duration
                };

                tasks.Add(animation.RunAsync(to, cancellationToken));
            }

            await Task.WhenAll(tasks);
            if (from != null && !cancellationToken.IsCancellationRequested) {
                from.IsVisible = false;
            }
        }

        protected static Visual GetVisualParent(Visual? from, Visual? to) {
            Visual? p1 = (from ?? to)!.GetVisualParent(),
                p2 = (to ?? from)!.GetVisualParent();

            if (p1 != null && p2 != null && p1 != p2) {
                throw new ArgumentException("Controls for Very must have same parent.");
            }

            return p1 ?? throw new InvalidOperationException("Cannot determine visual parent.");
        }
    }
}
