using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;

namespace WonderLab.Classes.Media.Animations;

public sealed class PageSlideFade(TimeSpan duration) : IPageTransition {
    public TimeSpan Duration { get; set; } = duration;
    
    public Easing SlideEasing { get; set; } = new CircularEaseInOut();

    public bool Fade { get; set; }
    
    public async Task Start(Visual? from, Visual? to, bool forward, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested) {
            return;
        }

        var tasks = new List<Task>();
        var parent = GetVisualParent(from, to);
        var distance = parent.Bounds.Height;
        var translateProperty = TranslateTransform.XProperty;

        if (from != null) {
            double end = forward ? -distance : distance;
            var animation = Fade ? new Animation {
                Easing = SlideEasing,
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
            } : new Animation {
                Easing = SlideEasing,
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

        if (to != null)
        {
            to.IsVisible = true;
            double end = forward ? distance : -distance;
            var animation = Fade ? new Animation {
                FillMode = FillMode.Forward,
                Easing = SlideEasing,
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
            } : new Animation {
                FillMode = FillMode.Forward,
                Easing = SlideEasing,
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

    private static Visual GetVisualParent(Visual? from, Visual? to)
    {
        var vp1 = (from ?? to)!.GetVisualParent();
        var vp2 = (to ?? from)!.GetVisualParent();

        if (vp1 != null && vp2 != null && vp1 != vp2)
        {
            throw new ArgumentException("Controls for PageSlide must have same parent.");
        }

        return vp1 ?? throw new InvalidOperationException("Cannot determine visual parent.");
    }
}