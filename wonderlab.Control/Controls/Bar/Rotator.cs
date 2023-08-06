using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Rendering;

// ReSharper disable ConvertToLambdaExpression

namespace wonderlab.control.Controls.Bar {
    public class Rotator : Panel {
        // Minimum speed
        // Rotator will stop if speed less than this constant value.
        // It is able to change in code-behind if this value is not satisfied.
        private static double _minimumSpeed = 0.0025;

        public static readonly DirectProperty<Rotator, double> SpeedProperty =
            AvaloniaProperty.RegisterDirect(nameof(Speed),
                rotator => rotator._speed,
                (Rotator rotator, double v) => {
                    if (rotator.IsEffectivelyVisible == false || rotator.IsEffectivelyEnabled == false)
                        return;

                    rotator._speed = v;
                    rotator.OnSpeedChanged(rotator, v);
                });
        private RotateTransform _renderTransform;
        private double _rotateDegree;
        private bool _running;
        private double _speed = 0.4;

        private Stopwatch _stopwatch = new();

        public Rotator() {
            RenderTransform = _renderTransform = new RotateTransform();
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static double MinimumSpeed
        {
            get => _minimumSpeed;
            set
            {
                if (value < 0) {
                    throw new ArgumentOutOfRangeException(nameof(value),
                        "MinimumSpeed should not less than zero. You can set it as zero, if you wish your rotator keep running.");
                }

                _minimumSpeed = value;
            }
        }

        public double Speed
        {
            get => _speed;
            set => SetAndRaise(SpeedProperty, ref _speed, value);
        }

        private void OnSpeedChanged(Rotator rotator, double newSpeed) {
            _rotateDegree += newSpeed * _stopwatch.Elapsed.TotalMilliseconds;
            _stopwatch.Restart();
            rotator._renderTransform.Angle = _rotateDegree % 360;
        }
    }
}