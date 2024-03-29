using Avalonia;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Avalonia.Interactivity;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using System;
using System.Collections;
using System.Reflection;

namespace WonderLab.Views.Controls;

public sealed class SmoothScrollContentPresenter : ScrollContentPresenter {
    private Vector _localOffset;
    private bool _isUpdatingByUser;
    private bool _isUpdatingByAnimation;

    public static readonly StyledProperty<Vector> AnimatableOffsetProperty;

    public Vector AnimatableOffset {
        get {
            return GetValue(AnimatableOffsetProperty);
        }
        private set {
            SetValue(AnimatableOffsetProperty, value);
        }
    }

    static SmoothScrollContentPresenter() {
        AnimatableOffsetProperty = AvaloniaProperty.Register<SmoothScrollContentPresenter, Vector>("AnimatableOffset");
        AnimatableOffsetProperty.Changed.AddClassHandler(delegate (SmoothScrollContentPresenter presenter, AvaloniaPropertyChangedEventArgs args) {
            if (!presenter._isUpdatingByUser) {
                if (args != null && args.Priority == BindingPriority.LocalValue && args.NewValue is Vector localOffset) {
                    presenter._localOffset = localOffset;
                }
                ScrollViewer scrollViewer = presenter.FindAncestorOfType<ScrollViewer>();
                presenter._isUpdatingByAnimation = true;
                presenter.SetCurrentValue(OffsetProperty, presenter.AnimatableOffset);
                if (scrollViewer != null) {
                    scrollViewer.Offset = presenter._localOffset;
                }
                presenter._isUpdatingByAnimation = false;
            }
        });
        ScrollViewer.OffsetProperty.Changed.AddClassHandler(delegate (SmoothScrollViewer viewer, AvaloniaPropertyChangedEventArgs args) {
            if (viewer.Presenter is SmoothScrollContentPresenter advanceScrollContentPresenter && !advanceScrollContentPresenter._isUpdatingByAnimation) {
                advanceScrollContentPresenter._isUpdatingByUser = true;
                advanceScrollContentPresenter._localOffset = viewer.Offset;
                advanceScrollContentPresenter.AnimatableOffset = viewer.Offset;
                advanceScrollContentPresenter._isUpdatingByUser = false;
            }
        });
    }

    public SmoothScrollContentPresenter() {
        TryRemoveRequestBringIntoViewEventHandler();
        AddHandler(Control.RequestBringIntoViewEvent, BringIntoViewRequested);
        base.Offset = default(Vector);
    }

    private void TryRemoveRequestBringIntoViewEventHandler() {
        (typeof(Interactive).GetField("_eventHandlers", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(this) as IDictionary)?.Remove(RequestBringIntoViewEvent);
    }

    private void BringIntoViewRequested(object sender, RequestBringIntoViewEventArgs e) {
        if (e.TargetObject != null) {
            e.Handled = BringDescendantIntoView(e.TargetObject, e.TargetRect);
        }
    }

    private new bool BringDescendantIntoView(Visual target, Rect targetRect) {
        Control? child = base.Child;
        if (child == null || !child.IsEffectivelyVisible) {
            return false;
        }
        if (base.Child is ILogicalScrollable logicalScrollable && logicalScrollable.IsLogicalScrollEnabled && target is Control control) {
            return logicalScrollable.BringIntoView(control, targetRect);
        }
        Matrix? matrix = target.TransformToVisual(base.Child);
        if (!matrix.HasValue) {
            return false;
        }
        Rect rect = targetRect.TransformToAABB(matrix.Value);
        Vector animatableOffset = base.Offset;
        bool flag = false;
        if (base.Offset.X + base.Viewport.Width > base.Child.Bounds.Width) {
            animatableOffset = animatableOffset.WithX(base.Child.Bounds.Width - base.Viewport.Width);
            flag = true;
        }
        if (base.Offset.Y + base.Viewport.Height > base.Child.Bounds.Height) {
            animatableOffset = animatableOffset.WithY(base.Child.Bounds.Height - base.Viewport.Height);
            flag = true;
        }
        if (rect.Bottom > animatableOffset.Y + base.Viewport.Height) {
            animatableOffset = animatableOffset.WithY(rect.Bottom - base.Viewport.Height + base.Child.Margin.Top);
            flag = true;
        }
        if (rect.Y < animatableOffset.Y) {
            animatableOffset = animatableOffset.WithY(rect.Y);
            flag = true;
        }
        if (rect.Right > animatableOffset.X + base.Viewport.Width) {
            animatableOffset = animatableOffset.WithX(rect.Right - base.Viewport.Width + base.Child.Margin.Left);
            flag = true;
        }
        if (rect.X < animatableOffset.X) {
            animatableOffset = animatableOffset.WithX(rect.X);
            flag = true;
        }
        if (flag) {
            AnimatableOffset = animatableOffset;
        }
        return flag;
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e) {
        if (!(base.TemplatedParent is SmoothScrollViewer smoothScrollViewer)) {
            base.OnPointerWheelChanged(e);
        } else if (base.Extent.Height > base.Viewport.Height || base.Extent.Width > base.Viewport.Width) {
            ILogicalScrollable logicalScrollable = base.Child as ILogicalScrollable;
            bool flag = logicalScrollable?.IsLogicalScrollEnabled ?? false;
            double num = base.Offset.X;
            double num2 = base.Offset.Y;
            if (base.Extent.Height > base.Viewport.Height) {
                double num3 = (flag ? logicalScrollable.ScrollSize.Height : smoothScrollViewer.SmoothScrollingStep);
                num2 += (0.0 - e.Delta.Y) * num3;
                num2 = Math.Max(num2, 0.0);
                num2 = Math.Min(num2, base.Extent.Height - base.Viewport.Height);
            }
            if (base.Extent.Width > base.Viewport.Width) {
                double num4 = (flag ? logicalScrollable.ScrollSize.Width : smoothScrollViewer.SmoothScrollingStep);
                num += (0.0 - e.Delta.X) * num4;
                num = Math.Max(num, 0.0);
                num = Math.Min(num, base.Extent.Width - base.Viewport.Width);
            }
            Vector vector = new Vector(num, num2);
            bool flag2 = vector != base.Offset;
            AnimatableOffset = vector;
            e.Handled = !base.IsScrollChainingEnabled || flag2;
        }
    }
}
