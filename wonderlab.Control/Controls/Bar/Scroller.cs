﻿using System;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace wonderlab.control.Controls.Bar
{
    public class Scroller : ContentControl, IScrollable, IScrollAnchorProvider
    {
        public static readonly DirectProperty<Scroller, bool> CanHorizontallyScrollProperty =
            AvaloniaProperty.RegisterDirect<Scroller, bool>(
                nameof(CanHorizontallyScroll),
                o => o.CanHorizontallyScroll);

        public static readonly DirectProperty<Scroller, bool> CanVerticallyScrollProperty =
            AvaloniaProperty.RegisterDirect<Scroller, bool>(
                nameof(CanVerticallyScroll),
                o => o.CanVerticallyScroll);

        public static readonly DirectProperty<Scroller, Size> ExtentProperty =
            AvaloniaProperty.RegisterDirect<Scroller, Size>(nameof(Extent),
                o => o.Extent,
                (o, v) => o.Extent = v);

        public static readonly DirectProperty<Scroller, Vector> OffsetProperty =
            AvaloniaProperty.RegisterDirect<Scroller, Vector>(
                nameof(Offset),
                o => o.Offset,
                (o, v) => o.Offset = v);

        public static readonly DirectProperty<Scroller, Size> ViewportProperty =
            AvaloniaProperty.RegisterDirect<Scroller, Size>(nameof(Viewport),
                o => o.Viewport,
                (o, v) => o.Viewport = v);

        public static readonly DirectProperty<Scroller, Size> LargeChangeProperty =
            AvaloniaProperty.RegisterDirect<Scroller, Size>(
                nameof(LargeChange),
                o => o.LargeChange);

        public static readonly DirectProperty<Scroller, Size> SmallChangeProperty =
            AvaloniaProperty.RegisterDirect<Scroller, Size>(
                nameof(SmallChange),
                o => o.SmallChange);

        public static readonly DirectProperty<Scroller, double> HorizontalScrollBarMaximumProperty =
            AvaloniaProperty.RegisterDirect<Scroller, double>(
                nameof(HorizontalScrollBarMaximum),
                o => o.HorizontalScrollBarMaximum);

        public static readonly DirectProperty<Scroller, double> HorizontalScrollBarValueProperty =
            AvaloniaProperty.RegisterDirect<Scroller, double>(
                nameof(HorizontalScrollBarValue),
                o => o.HorizontalScrollBarValue,
                (o, v) => o.HorizontalScrollBarValue = v);

        public static readonly DirectProperty<Scroller, double> HorizontalScrollBarViewportSizeProperty =
            AvaloniaProperty.RegisterDirect<Scroller, double>(
                nameof(HorizontalScrollBarViewportSize),
                o => o.HorizontalScrollBarViewportSize);

        public static readonly AttachedProperty<ScrollBarVisibility> HorizontalScrollBarVisibilityProperty =
            AvaloniaProperty.RegisterAttached<Scroller, Avalonia.Controls.Control, ScrollBarVisibility>(
                nameof(HorizontalScrollBarVisibility),
                ScrollBarVisibility.Hidden);

        public static readonly DirectProperty<Scroller, double> VerticalScrollBarMaximumProperty =
            AvaloniaProperty.RegisterDirect<Scroller, double>(
                nameof(VerticalScrollBarMaximum),
                o => o.VerticalScrollBarMaximum);

        public static readonly DirectProperty<Scroller, double> VerticalScrollBarValueProperty =
            AvaloniaProperty.RegisterDirect<Scroller, double>(
                nameof(VerticalScrollBarValue),
                o => o.VerticalScrollBarValue,
                (o, v) => o.VerticalScrollBarValue = v);

        public static readonly DirectProperty<Scroller, double> VerticalScrollBarViewportSizeProperty =
            AvaloniaProperty.RegisterDirect<Scroller, double>(
                nameof(VerticalScrollBarViewportSize),
                o => o.VerticalScrollBarViewportSize);

        public static readonly AttachedProperty<ScrollBarVisibility> VerticalScrollBarVisibilityProperty =
            AvaloniaProperty.RegisterAttached<Scroller, Avalonia.Controls.Control, ScrollBarVisibility>(
                nameof(VerticalScrollBarVisibility),
                ScrollBarVisibility.Auto);

        public static readonly DirectProperty<Scroller, bool> IsExpandedProperty =
            ScrollBar.IsExpandedProperty.AddOwner<Scroller>(o => o.IsExpanded);

        public static readonly StyledProperty<bool> AllowAutoHideProperty =
            ScrollBar.AllowAutoHideProperty.AddOwner<Scroller>();

        public static readonly RoutedEvent<ScrollChangedEventArgs> ScrollChangedEvent =
            RoutedEvent.Register<Scroller, ScrollChangedEventArgs>(
                nameof(ScrollChanged),
                RoutingStrategies.Bubble);
        
        /// <summary>
        /// Defines the <see cref="CanScrollLeft"/> property.
        /// </summary>
        public static readonly DirectProperty<Scroller, bool> CanScrollLeftProperty =
            AvaloniaProperty.RegisterDirect<Scroller, bool>(
                nameof(CanScrollLeft),
                o => o.CanScrollLeft);
        
        /// <summary>
        /// Defines the <see cref="CanScrollLeft"/> property.
        /// </summary>
        public static readonly DirectProperty<Scroller, bool> CanScrollRightProperty =
            AvaloniaProperty.RegisterDirect<Scroller, bool>(
                nameof(CanScrollRight),
                o => o.CanScrollRight);

        internal const double DefaultSmallChange = 16;

        private IDisposable _childSubscription;
        private ILogicalScrollable _logicalScrollable;
        private Size _extent;
        private Vector _offset;
        private Size _viewport;
        private Size _oldExtent;
        private Vector _oldOffset;
        private Size _oldViewport;
        private Size _largeChange;
        private Size _smallChange = new Size(DefaultSmallChange, DefaultSmallChange);
        private bool _isExpanded;
        private bool _canScrollLeft;
        private bool _canScrollRight;
        private IDisposable _scrollBarExpandSubscription;

        /// <summary>
        /// Initializes static members of the <see cref="Scroller"/> class.
        /// </summary>
        static Scroller()
        {
            HorizontalScrollBarVisibilityProperty.Changed.AddClassHandler<Scroller>((x, e) => x.ScrollBarVisibilityChanged(e));
            VerticalScrollBarVisibilityProperty.Changed.AddClassHandler<Scroller>((x, e) => x.ScrollBarVisibilityChanged(e));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scroller"/> class.
        /// </summary>
        public Scroller()
        {
            LayoutUpdated += OnLayoutUpdated;
        }

        /// <summary>
        /// Occurs when changes are detected to the scroll position, extent, or viewport size.
        /// </summary>
        public event EventHandler<ScrollChangedEventArgs> ScrollChanged
        {
            add => AddHandler(ScrollChangedEvent, value);
            remove => RemoveHandler(ScrollChangedEvent, value);
        }

        /// <summary>
        /// Gets the extent of the scrollable content.
        /// </summary>
        public Size Extent
        {
            get
            {
                return _extent;
            }

            private set
            {
                if (SetAndRaise(ExtentProperty, ref _extent, value))
                {
                    CalculatedPropertiesChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the current scroll offset.
        /// </summary>
        public Vector Offset
        {
            get
            {
                return _offset;
            }

            set
            {
                if (SetAndRaise(OffsetProperty, ref _offset, CoerceOffset(Extent, Viewport, value)))
                {
                    CalculatedPropertiesChanged();
                }
            }
        }

        /// <summary>
        /// Gets the size of the viewport on the scrollable content.
        /// </summary>
        public Size Viewport
        {
            get
            {
                return _viewport;
            }

            private set
            {
                if (SetAndRaise(ViewportProperty, ref _viewport, value))
                {
                    CalculatedPropertiesChanged();
                }
            }
        }

        /// <summary>
        /// Gets the large (page) change value for the scroll viewer.
        /// </summary>
        public Size LargeChange => _largeChange;

        /// <summary>
        /// Gets the small (line) change value for the scroll viewer.
        /// </summary>
        public Size SmallChange => _smallChange;

        /// <summary>
        /// Gets or sets the horizontal scrollbar visibility.
        /// </summary>
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return GetValue(HorizontalScrollBarVisibilityProperty); }
            set { SetValue(HorizontalScrollBarVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the vertical scrollbar visibility.
        /// </summary>
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return GetValue(VerticalScrollBarVisibilityProperty); }
            set { SetValue(VerticalScrollBarVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets a value indicating whether the viewer can scroll horizontally.
        /// </summary>
        protected bool CanHorizontallyScroll
        {
            get { return HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled; }
        }

        /// <summary>
        /// Gets a value indicating whether the viewer can scroll vertically.
        /// </summary>
        protected bool CanVerticallyScroll
        {
            get { return VerticalScrollBarVisibility != ScrollBarVisibility.Disabled; }
        }

        /// <inheritdoc/>
        public IControl CurrentAnchor => (Presenter as IScrollAnchorProvider)?.CurrentAnchor;

        /// <summary>
        /// Gets the maximum horizontal scrollbar value.
        /// </summary>
        protected double HorizontalScrollBarMaximum
        {
            get { return Max(_extent.Width - _viewport.Width, 0); }
        }

        /// <summary>
        /// Gets or sets the horizontal scrollbar value.
        /// </summary>
        protected double HorizontalScrollBarValue
        {
            get { return _offset.X; }
            set
            {
                if (_offset.X != value)
                {
                    var old = Offset.X;
                    Offset = Offset.WithX(value);
                    RaisePropertyChanged(HorizontalScrollBarValueProperty, old, value);
                }
            }
        }

        /// <summary>
        /// Gets the size of the horizontal scrollbar viewport.
        /// </summary>
        protected double HorizontalScrollBarViewportSize
        {
            get { return _viewport.Width; }
        }

        /// <summary>
        /// Gets the maximum vertical scrollbar value.
        /// </summary>
        protected double VerticalScrollBarMaximum
        {
            get { return Max(_extent.Height - _viewport.Height, 0); }
        }

        /// <summary>
        /// Gets or sets the vertical scrollbar value.
        /// </summary>
        protected double VerticalScrollBarValue
        {
            get { return _offset.Y; }
            set
            {
                if (_offset.Y != value)
                {
                    var old = Offset.Y;
                    Offset = Offset.WithY(value);
                    RaisePropertyChanged(VerticalScrollBarValueProperty, old, value);
                }
            }
        }

        /// <summary>
        /// Gets the size of the vertical scrollbar viewport.
        /// </summary>
        protected double VerticalScrollBarViewportSize
        {
            get { return _viewport.Height; }
        }

        /// <summary>
        /// Gets a value that indicates whether any scrollbar is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get => _isExpanded;
            private set => SetAndRaise(ScrollBar.IsExpandedProperty, ref _isExpanded, value);
        }

        /// <summary>
        /// Gets a value that indicates whether scrollbars can hide itself when user is not interacting with it.
        /// </summary>
        public bool AllowAutoHide
        {
            get => GetValue(AllowAutoHideProperty);
            set => SetValue(AllowAutoHideProperty, value);
        }
        
        
        /// <summary>
        ///
        /// </summary>
        public bool CanScrollLeft
        {
            get => _canScrollLeft;
            private set => _canScrollLeft = value;
        }
        
        /// <summary>
        ///
        /// </summary>
        public bool CanScrollRight
        {
            get => _canScrollRight;
            private set => _canScrollRight = value;
        }
        

        /// <summary>
        /// Scrolls the content up one line.
        /// </summary>
        public void LineUp()
        {
            Offset -= new Vector(0, _smallChange.Height);
        }

        /// <summary>
        /// Scrolls the content down one line.
        /// </summary>
        public void LineDown()
        {
            Offset += new Vector(0, _smallChange.Height);
        }

        /// <summary>
        /// Scrolls the content left one line.
        /// </summary>
        public void LineLeft()
        {
            Offset -= new Vector(_smallChange.Width, 0);
        }

        /// <summary>
        /// Scrolls the content right one line.
        /// </summary>
        public void LineRight()
        {
            Offset += new Vector(_smallChange.Width, 0);
        }
        
        /// <summary>
        /// Scrolls the content upward by half page.
        /// </summary>
        public void OneByThirdPageUp()
        {
            VerticalScrollBarValue = Math.Max(_offset.Y - _viewport.Height / 3.0, 0);
        }

        /// <summary>
        /// Scrolls the content downward by half page.
        /// </summary>
        public void OneByThirdPageDown()
        {
            VerticalScrollBarValue = Math.Min(_offset.Y + _viewport.Height / 3.0, VerticalScrollBarMaximum);
        }

        /// <summary>
        /// Scrolls the content left by half page.
        /// </summary>
        public void OneByThirdPageLeft()
        {
            HorizontalScrollBarValue = Math.Max(_offset.X - _viewport.Width / 3.0, 0);
        }

        /// <summary>
        /// Scrolls the content tight by half page.
        /// </summary>
        public void OneByThirdPageRight()
        {
            HorizontalScrollBarValue = Math.Min(_offset.X + _viewport.Width / 3.0, HorizontalScrollBarMaximum);
        }
        
        /// <summary>
        /// Scrolls the content upward by half page.
        /// </summary>
        public void HalfPageUp()
        {
            VerticalScrollBarValue = Math.Max(_offset.Y - _viewport.Height / 2.0, 0);
        }

        /// <summary>
        /// Scrolls the content downward by half page.
        /// </summary>
        public void HalfPageDown()
        {
            VerticalScrollBarValue = Math.Min(_offset.Y + _viewport.Height / 2.0, VerticalScrollBarMaximum);
        }

        /// <summary>
        /// Scrolls the content left by half page.
        /// </summary>
        public void HalfPageLeft()
        {
            HorizontalScrollBarValue = Math.Max(_offset.X - _viewport.Width / 2.0, 0);
        }

        /// <summary>
        /// Scrolls the content tight by half page.
        /// </summary>
        public void HalfPageRight()
        {
            HorizontalScrollBarValue = Math.Min(_offset.X + _viewport.Width / 2.0, HorizontalScrollBarMaximum);
        }

        /// <summary>
        /// Scrolls the content upward by one page.
        /// </summary>
        public void PageUp()
        {
            VerticalScrollBarValue = Math.Max(_offset.Y - _viewport.Height, 0);
        }

        /// <summary>
        /// Scrolls the content downward by one page.
        /// </summary>
        public void PageDown()
        {
            VerticalScrollBarValue = Math.Min(_offset.Y + _viewport.Height, VerticalScrollBarMaximum);
        }

        /// <summary>
        /// Scrolls the content left by one page.
        /// </summary>
        public void PageLeft()
        {
            HorizontalScrollBarValue = Math.Max(_offset.X - _viewport.Width, 0);
        }

        /// <summary>
        /// Scrolls the content tight by one page.
        /// </summary>
        public void PageRight()
        {
            HorizontalScrollBarValue = Math.Min(_offset.X + _viewport.Width, HorizontalScrollBarMaximum);
        }

        /// <summary>
        /// Scrolls to the top-left corner of the content.
        /// </summary>
        public void ScrollToHome()
        {
            Offset = new Vector(double.NegativeInfinity, double.NegativeInfinity);
        }

        /// <summary>
        /// Scrolls to the bottom-left corner of the content.
        /// </summary>
        public void ScrollToEnd()
        {
            Offset = new Vector(double.NegativeInfinity, double.PositiveInfinity);
        }

        /// <summary>
        /// Gets the value of the HorizontalScrollBarVisibility attached property.
        /// </summary>
        /// <param name="control">The control to read the value from.</param>
        /// <returns>The value of the property.</returns>
        public static ScrollBarVisibility GetHorizontalScrollBarVisibility(Avalonia.Controls.Control control)
        {
            return control.GetValue(HorizontalScrollBarVisibilityProperty);
        }

        /// <summary>
        /// Gets the value of the HorizontalScrollBarVisibility attached property.
        /// </summary>
        /// <param name="control">The control to set the value on.</param>
        /// <param name="value">The value of the property.</param>
        public static void SetHorizontalScrollBarVisibility(Avalonia.Controls.Control control, ScrollBarVisibility value)
        {
            control.SetValue(HorizontalScrollBarVisibilityProperty, value);
        }

        /// <summary>
        /// Gets the value of the VerticalScrollBarVisibility attached property.
        /// </summary>
        /// <param name="control">The control to read the value from.</param>
        /// <returns>The value of the property.</returns>
        public static ScrollBarVisibility GetVerticalScrollBarVisibility(Avalonia.Controls.Control control)
        {
            return control.GetValue(VerticalScrollBarVisibilityProperty);
        }

        /// <summary>
        /// Gets the value of the VerticalScrollBarVisibility attached property.
        /// </summary>
        /// <param name="control">The control to set the value on.</param>
        /// <param name="value">The value of the property.</param>
        public static void SetVerticalScrollBarVisibility(Avalonia.Controls.Control control, ScrollBarVisibility value)
        {
            control.SetValue(VerticalScrollBarVisibilityProperty, value);
        }

        /// <inheritdoc/>
        public void RegisterAnchorCandidate(IControl element)
        {
            (Presenter as IScrollAnchorProvider)?.RegisterAnchorCandidate(element);
        }

        /// <inheritdoc/>
        public void UnregisterAnchorCandidate(IControl element)
        {
            (Presenter as IScrollAnchorProvider)?.UnregisterAnchorCandidate(element);
        }

        protected override bool RegisterContentPresenter(IContentPresenter presenter)
        {
            _childSubscription?.Dispose();
            _childSubscription = null;

            if (base.RegisterContentPresenter(presenter))
            {
                _childSubscription = Presenter?
                    .GetObservable(ContentPresenter.ChildProperty)
                    .Subscribe(ChildChanged);
                return true;
            }

            return false;
        }

        internal static Vector CoerceOffset(Size extent, Size viewport, Vector offset)
        {
            var maxX = Math.Max(extent.Width - viewport.Width, 0);
            var maxY = Math.Max(extent.Height - viewport.Height, 0);
            return new Vector(Clamp(offset.X, 0, maxX), Clamp(offset.Y, 0, maxY));
        }

        private static double Clamp(double value, double min, double max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        private static double Max(double x, double y)
        {
            var result = Math.Max(x, y);
            return double.IsNaN(result) ? 0 : result;
        }

        private void ChildChanged(IControl child)
        {
            if (_logicalScrollable is object)
            {
                _logicalScrollable.ScrollInvalidated -= LogicalScrollInvalidated;
                _logicalScrollable = null;
            }

            if (child is ILogicalScrollable logical)
            {
                _logicalScrollable = logical;
                logical.ScrollInvalidated += LogicalScrollInvalidated;
            }

            CalculatedPropertiesChanged();
        }

        private void LogicalScrollInvalidated(object sender, EventArgs e)
        {
            CalculatedPropertiesChanged();
        }

        private void ScrollBarVisibilityChanged(AvaloniaPropertyChangedEventArgs e)
        {
            var wasEnabled = !ScrollBarVisibility.Disabled.Equals(e.OldValue);
            var isEnabled = !ScrollBarVisibility.Disabled.Equals(e.NewValue);

            if (wasEnabled != isEnabled)
            {
                if (e.Property == HorizontalScrollBarVisibilityProperty)
                {
                    RaisePropertyChanged(
                        CanHorizontallyScrollProperty,
                        wasEnabled,
                        isEnabled);
                }
                else if (e.Property == VerticalScrollBarVisibilityProperty)
                {
                    RaisePropertyChanged(
                        CanVerticallyScrollProperty,
                        wasEnabled,
                        isEnabled);
                }
            }
        }

        private void CalculatedPropertiesChanged()
        {
            // Pass old values of 0 here because we don't have the old values at this point,
            // and it shouldn't matter as only the template uses these properies.
            RaisePropertyChanged(HorizontalScrollBarMaximumProperty, 0, HorizontalScrollBarMaximum);
            RaisePropertyChanged(HorizontalScrollBarValueProperty, 0, HorizontalScrollBarValue);
            RaisePropertyChanged(HorizontalScrollBarViewportSizeProperty, 0, HorizontalScrollBarViewportSize);
            RaisePropertyChanged(VerticalScrollBarMaximumProperty, 0, VerticalScrollBarMaximum);
            RaisePropertyChanged(VerticalScrollBarValueProperty, 0, VerticalScrollBarValue);
            RaisePropertyChanged(VerticalScrollBarViewportSizeProperty, 0, VerticalScrollBarViewportSize);

            if (_logicalScrollable?.IsLogicalScrollEnabled == true)
            {
                SetAndRaise(SmallChangeProperty, ref _smallChange, _logicalScrollable.ScrollSize);
                SetAndRaise(LargeChangeProperty, ref _largeChange, _logicalScrollable.PageScrollSize);
            }
            else
            {
                SetAndRaise(SmallChangeProperty, ref _smallChange, new Size(DefaultSmallChange, DefaultSmallChange));
                SetAndRaise(LargeChangeProperty, ref _largeChange, Viewport);
            }

            CanScrollLeft = _offset.X > 0.0;
            CanScrollRight = _offset.X < HorizontalScrollBarMaximum;
            RaisePropertyChanged(CanScrollLeftProperty, false, CanScrollLeft);
            RaisePropertyChanged(CanScrollRightProperty, false, CanScrollRight);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.PageUp)
            {
                PageUp();
                e.Handled = true;
            }
            else if (e.Key == Key.PageDown)
            {
                PageDown();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Called when a change in scrolling state is detected, such as a change in scroll
        /// position, extent, or viewport size.
        /// </summary>
        /// <param name="e">The event args.</param>
        /// <remarks>
        /// If you override this method, call `base.OnScrollChanged(ScrollChangedEventArgs)` to
        /// ensure that this event is raised.
        /// </remarks>
        protected virtual void OnScrollChanged(ScrollChangedEventArgs e)
        {
            RaiseEvent(e);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _scrollBarExpandSubscription?.Dispose();

            _scrollBarExpandSubscription = SubscribeToScrollBars(e);
        }

        private IDisposable SubscribeToScrollBars(TemplateAppliedEventArgs e)
        {
            static IObservable<bool> GetExpandedObservable(ScrollBar scrollBar)
            {
                return scrollBar?.GetObservable(ScrollBar.IsExpandedProperty);
            }

            var horizontalScrollBar = e.NameScope.Find<ScrollBar>("PART_HorizontalScrollBar");
            var verticalScrollBar = e.NameScope.Find<ScrollBar>("PART_VerticalScrollBar");

            var horizontalExpanded = GetExpandedObservable(horizontalScrollBar);
            var verticalExpanded = GetExpandedObservable(verticalScrollBar);

            IObservable<bool> actualExpanded = null;

            if (horizontalExpanded != null && verticalExpanded != null)
            {
                actualExpanded = horizontalExpanded.CombineLatest(verticalExpanded, (h, v) => h || v);
            }
            else
            {
                if (horizontalExpanded != null)
                {
                    actualExpanded = horizontalExpanded;
                }
                else if (verticalExpanded != null)
                {
                    actualExpanded = verticalExpanded;
                }
            }

            return actualExpanded?.Subscribe(OnScrollBarExpandedChanged);
        }

        private void OnScrollBarExpandedChanged(bool isExpanded)
        {
            IsExpanded = isExpanded;
        }

        private void OnLayoutUpdated(object sender, EventArgs e) => RaiseScrollChanged();

        private void RaiseScrollChanged()
        {
            var extentDelta = new Vector(Extent.Width - _oldExtent.Width, Extent.Height - _oldExtent.Height);
            var offsetDelta = Offset - _oldOffset;
            var viewportDelta = new Vector(Viewport.Width - _oldViewport.Width, Viewport.Height - _oldViewport.Height);

            if (!extentDelta.NearlyEquals(default) || !offsetDelta.NearlyEquals(default) || !viewportDelta.NearlyEquals(default))
            {
                var e = new ScrollChangedEventArgs(extentDelta, offsetDelta, viewportDelta);
                OnScrollChanged(e);

                _oldExtent = Extent;
                _oldOffset = Offset;
                _oldViewport = Viewport;
            }
        }
    }
}