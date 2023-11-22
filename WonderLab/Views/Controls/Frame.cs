using System;
using Avalonia;
using System.IO;
using System.Text;
using System.Threading;
using Avalonia.Logging;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Collections;
using Avalonia.Interactivity;
using WonderLab.Classes.Enums;
using WonderLab.Classes.Medias;
using System.Collections.Generic;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using System.Collections.Specialized;
using WonderLab.Classes.Interfaces.Navigation;

namespace WonderLab.Views.Controls
{
    public delegate void NavigatedEventHandler(object sender, NavigationEventArgs e);

    public delegate void NavigationStoppedEventHandler(object sender, NavigationEventArgs e);

    public delegate void NavigationFailedEventHandler(object sender, NavigationFailedEventArgs e);

    public delegate void NavigatingCancelEventHandler(object sender, NavigatingCancelEventArgs e);

    [TemplatePart(s_tpContentPresenter, typeof(ContentPresenter))]
    public class Frame : ContentControl {
        private IList<PageStackEntry> _backStack;

        private IList<PageStackEntry> _forwardStack;

        private INavigationPageFactory _pageFactory;

        public static readonly StyledProperty<Type> SourcePageTypeProperty =
            AvaloniaProperty.Register<Frame, Type>(nameof(SourcePageType));

        public static readonly StyledProperty<int> CacheSizeProperty =
            AvaloniaProperty.Register<Frame, int>(nameof(CacheSize),
                defaultValue: 10,
                coerce: (x, v) => v >= 0 ? v : 0);

        public static readonly DirectProperty<Frame, int> BackStackDepthProperty =
            AvaloniaProperty.RegisterDirect<Frame, int>(nameof(BackStackDepth),
                x => x.BackStackDepth);

        public static readonly DirectProperty<Frame, bool> CanGoBackProperty =
            AvaloniaProperty.RegisterDirect<Frame, bool>(nameof(CanGoBack),
                x => x.CanGoBack);

        public static readonly DirectProperty<Frame, bool> CanGoForwardProperty =
            AvaloniaProperty.RegisterDirect<Frame, bool>(nameof(CanGoForward),
                x => x.CanGoForward);

        public static readonly DirectProperty<Frame, Type> CurrentSourcePageTypeProperty =
            AvaloniaProperty.RegisterDirect<Frame, Type>(nameof(CurrentSourcePageType),
                x => x.CurrentSourcePageType);

        public static readonly DirectProperty<Frame, IList<PageStackEntry>> BackStackProperty =
            AvaloniaProperty.RegisterDirect<Frame, IList<PageStackEntry>>(nameof(BackStack),
                x => x.BackStack);

        public static readonly DirectProperty<Frame, IList<PageStackEntry>> ForwardStackProperty =
            AvaloniaProperty.RegisterDirect<Frame, IList<PageStackEntry>>(nameof(ForwardStack),
                x => x.ForwardStack);

        public static readonly StyledProperty<bool> IsNavigationStackEnabledProperty =
            AvaloniaProperty.Register<Frame, bool>(nameof(IsNavigationStackEnabled),
                defaultValue: true);

        public static readonly DirectProperty<Frame, INavigationPageFactory> NavigationPageFactoryProperty =
            AvaloniaProperty.RegisterDirect<Frame, INavigationPageFactory>(nameof(NavigationPageFactory),
                x => x.NavigationPageFactory, (x, v) => x.NavigationPageFactory = v);

        public Type SourcePageType
        {
            get => GetValue(SourcePageTypeProperty);
            set => SetValue(SourcePageTypeProperty, value);
        }

        public int CacheSize
        {
            get => GetValue(CacheSizeProperty);
            set => SetValue(CacheSizeProperty, value);
        }

        public int BackStackDepth
        {
            get => _backStack.Count;
        }

        public bool CanGoBack => _backStack.Count > 0;

        public bool CanGoForward => _forwardStack.Count > 0;

        public Type CurrentSourcePageType => Content?.GetType();

        public IList<PageStackEntry> BackStack
        {
            get => _backStack;
            private set => SetAndRaise(BackStackProperty, ref _backStack, value);
        }

        public IList<PageStackEntry> ForwardStack
        {
            get => _forwardStack;
            private set => SetAndRaise(ForwardStackProperty, ref _forwardStack, value);
        }

        public bool IsNavigationStackEnabled
        {
            get => GetValue(IsNavigationStackEnabledProperty);
            set => SetValue(IsNavigationStackEnabledProperty, value);
        }

        public INavigationPageFactory NavigationPageFactory
        {
            get => _pageFactory;
            set => SetAndRaise(NavigationPageFactoryProperty, ref _pageFactory, value);
        }

        internal PageStackEntry CurrentEntry { get; set; }

        public event NavigatedEventHandler Navigated;

        public event NavigatingCancelEventHandler Navigating;

        public event NavigationFailedEventHandler NavigationFailed;

        public event NavigationStoppedEventHandler NavigationStopped;

        public static readonly RoutedEvent<NavigatingCancelEventArgs> NavigatingFromEvent =
            RoutedEvent.Register<Control, NavigatingCancelEventArgs>("NavigatingFrom",
                RoutingStrategies.Direct);

        public static readonly RoutedEvent<NavigationEventArgs> NavigatedFromEvent =
            RoutedEvent.Register<Control, NavigationEventArgs>("NavigatedFrom",
                RoutingStrategies.Direct);

        public static readonly RoutedEvent<NavigationEventArgs> NavigatedToEvent =
            RoutedEvent.Register<Control, NavigationEventArgs>("NavigatedTo",
                RoutingStrategies.Direct);

        public Frame() {
            var back = new AvaloniaList<PageStackEntry>();
            var forw = new AvaloniaList<PageStackEntry>();

            back.CollectionChanged += OnBackStackChanged;
            forw.CollectionChanged += OnForwardStackChanged;

            BackStack = back;
            ForwardStack = forw;
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
            base.OnPropertyChanged(change);
            if (change.Property == ContentProperty) {
                if (change.NewValue == null) {
                    CurrentEntry = null;
                }
            } else if (change.Property == SourcePageTypeProperty) {
                if (!_isNavigating) {
                    if (change.NewValue is null)
                        throw new InvalidOperationException("SourcePageType cannot be null. Use Content instead.");

                    Navigate(change.GetNewValue<Type>());
                }
            } else if (change.Property == IsNavigationStackEnabledProperty) {
                if (!change.GetNewValue<bool>()) {
                    _backStack.Clear();
                    _forwardStack.Clear();
                    _pageCache.Clear();
                }
            }
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
            base.OnApplyTemplate(e);
            _presenter = e.NameScope.Find<ContentPresenter>(s_tpContentPresenter);
        }

        protected override bool RegisterContentPresenter(ContentPresenter presenter) {
            if (presenter.Name == "ContentPresenter")
                return true;

            return base.RegisterContentPresenter(presenter);
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e) {
            base.OnAttachedToVisualTree(e);

            if (e.Root is TopLevel tl) {
                tl.BackRequested += OnTopLevelBackRequested;
            }
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e) {
            base.OnDetachedFromVisualTree(e);

            if (e.Root is TopLevel tl) {
                tl.BackRequested -= OnTopLevelBackRequested;
            }
        }

        public void GoBack() => GoBack(null);

        public void GoBack(NavigationTransitionInfo infoOverride) {
            if (CanGoBack) {
                var entry = _backStack[_backStack.Count - 1];
                if (infoOverride != null) {
                    entry.NavigationTransitionInfo = infoOverride;
                } else {
                    entry.NavigationTransitionInfo = CurrentEntry?.NavigationTransitionInfo ?? null;
                }

                NavigateCore(entry, NavigationMode.Back);
            }
        }

        public void GoForward() {
            if (CanGoForward) {
                NavigateCore(_forwardStack[_forwardStack.Count - 1], NavigationMode.Forward);
            }
        }

        public bool Navigate(Type sourcePageType) => Navigate(sourcePageType, null, null);

        public bool Navigate(Type sourcePageType, object parameter) => Navigate(sourcePageType, parameter, null);

        public bool Navigate(Type sourcePageType, object parameter, NavigationTransitionInfo infoOverride) {
            return NavigateCore(new PageStackEntry(sourcePageType, parameter,
                infoOverride), NavigationMode.New);
        }

        public bool NavigateToType(Type sourcePageType, object parameter, FrameNavigationOptions navOptions) =>
            NavigateCore(new PageStackEntry(sourcePageType, parameter, navOptions?.TransitionInfoOverride),
                NavigationMode.New, navOptions);

        public bool NavigateFromObject(object target, FrameNavigationOptions navOptions = null) {
            // Check the cache first to see if we have an existing page that matches
            // For this check we check by both type and object reference
            var existing = CheckCacheAndGetPage(null, target);

            if (existing == null) {
                // If we don't have a previous reference, try to resolve via Factory
                existing = NavigationPageFactory.GetPageFromObject(target);

                // Unable to locate page, return false
                if (existing == null)
                    return false;
            }

            // The page source Type here will be whatever was specified as 'target'
            var entry = new PageStackEntry(target.GetType(), null, navOptions?.TransitionInfoOverride) {
                Instance = existing,
                Context = target
            };

            return NavigateCore(entry, NavigationMode.New, navOptions);
        }

        public string GetNavigationState() {
            if (!IsNavigationStackEnabled) {
                throw new InvalidOperationException("Cannot retreive navigation stack when IsNavigationStackEnabled is false");
            }

            static void AppendEntry(StringBuilder sb, PageStackEntry entry) {
                sb.Append(entry.SourcePageType.AssemblyQualifiedName);
                sb.Append('|');
                if (entry.Parameter != null) {
                    sb.Append(entry.Parameter.ToString());
                }
                sb.AppendLine();
            }

            var sb = new StringBuilder();

            if (CurrentEntry != null) {
                AppendEntry(sb, CurrentEntry);
            }

            sb.AppendLine(BackStackDepth.ToString());

            for (int i = 0; i < BackStackDepth; i++) {
                AppendEntry(sb, BackStack[i]);
            }

            sb.AppendLine(ForwardStack.Count.ToString());

            for (int i = 0; i < ForwardStack.Count; i++) {
                AppendEntry(sb, ForwardStack[i]);
            }

            return sb.ToString();
        }

        public void SetNavigationState(string navState) =>
            SetNavigationState(navState, false);

        public void SetNavigationState(string navState, bool suppressNavigate) {
            if (!IsNavigationStackEnabled)
                throw new InvalidOperationException("Cannot set navigation stack when IsNavigationStackEnabled is false");

            BackStack.Clear();
            ForwardStack.Clear();
            CurrentEntry = null;
            Content = null;
            _pageCache.Clear();

            using (var reader = new StringReader(navState)) {
                var firstLine = reader.ReadLine(); // Current Page

                bool addCurrentEntryToBackStack = false;
                if (firstLine[0] != '|') {
                    var indexOfSep = firstLine.IndexOf('|');
                    var pageType = Type.GetType(firstLine.Substring(0, indexOfSep));
                    var param = firstLine.Substring(indexOfSep + 1);
                    CurrentEntry = new PageStackEntry(pageType, param, null);

                    if (!suppressNavigate) {
                        var page = CreatePageAndCacheIfNecessary(pageType);
                        CurrentEntry.Instance = page;

                        SetContentAndAnimate(CurrentEntry);
                        page.RaiseEvent(new NavigationEventArgs(page, NavigationMode.New, null, param, pageType) { RoutedEvent = NavigatedToEvent });
                    } else {
                        addCurrentEntryToBackStack = true;
                    }
                }

                var numBackLine = int.Parse(reader.ReadLine());

                for (int i = 0; i < numBackLine; i++) {
                    var line = reader.ReadLine();
                    var indexOfSep = line.IndexOf('|');
                    var pageType = Type.GetType(line.Substring(0, indexOfSep));

                    if (pageType == null) {
                        // Don't fail if we get an invalid page, log & continue
                        Logger.TryGet(LogEventLevel.Error, "Frame")?
                            .Log("Frame", $"Attempting to parse the type '{line.Substring(0, indexOfSep)}' failed. Page was skipped");

                        continue;
                    }

                    var param = line.Substring(indexOfSep + 1);

                    var entry = new PageStackEntry(pageType, param, null);
                    BackStack.Add(entry);
                }

                if (addCurrentEntryToBackStack) {
                    BackStack.Add(CurrentEntry);
                    CurrentEntry = null;
                }

                var numForwardLine = int.Parse(reader.ReadLine());

                for (int i = 0; i < numForwardLine; i++) {
                    var line = reader.ReadLine();
                    var indexOfSep = line.IndexOf('|');
                    var pageType = Type.GetType(line.Substring(0, indexOfSep));
                    var param = line.Substring(indexOfSep + 1);

                    if (pageType == null) {
                        // Don't fail if we get an invalid page, log & continue
                        Logger.TryGet(LogEventLevel.Error, "Frame")?
                            .Log("Frame", $"Attempting to parse the type '{line.Substring(0, indexOfSep)}' failed. Page was skipped");

                        continue;
                    }

                    var entry = new PageStackEntry(pageType, param, null);
                    ForwardStack.Add(entry);
                }
            }
        }

        private bool NavigateCore(PageStackEntry entry, NavigationMode mode, FrameNavigationOptions options = null) {
            try {
                _isNavigating = true;

                var ea = new NavigatingCancelEventArgs(mode,
                    entry.NavigationTransitionInfo,
                    entry.Parameter,
                    entry.SourcePageType);

                Navigating?.Invoke(this, ea);

                if (ea.Cancel) {
                    OnNavigationStopped(entry, mode);
                    return false;
                }

                if (CurrentEntry?.Instance is Control oldPage) {
                    ea.RoutedEvent = NavigatingFromEvent;
                    oldPage.RaiseEvent(ea);

                    if (ea.Cancel) {
                        OnNavigationStopped(entry, mode);
                        return false;
                    }
                }

                var prevEntry = CurrentEntry;
                bool wasPageSet = entry.Instance != null;

                if (mode == NavigationMode.New && !wasPageSet) {
                    entry.Instance = CheckCacheAndGetPage(entry.SourcePageType);
                }

                if (entry.Instance == null) {
                    var page = CreatePageAndCacheIfNecessary(entry.SourcePageType);
                    if (page == null) {
                        throw new ArgumentException($"The type {entry.SourcePageType} is not a valid page type.");
                    }

                    entry.Instance = page;
                } else if (wasPageSet) {
                    TryAddToCache(entry.Context, entry.Instance);
                }

                var oldEntry = CurrentEntry;
                CurrentEntry = entry;

                var navEA = new NavigationEventArgs(
                    CurrentEntry.Instance,
                    mode, entry.NavigationTransitionInfo,
                    entry.Parameter,
                    entry.SourcePageType);

                if (oldEntry != null) {
                    navEA.RoutedEvent = NavigatedFromEvent;
                    oldEntry.Instance.RaiseEvent(navEA);
                }

                SetContentAndAnimate(entry);

                bool addToNavStack = options?.IsNavigationStackEnabled ?? IsNavigationStackEnabled;

                if (addToNavStack) {
                    switch (mode) {
                        case NavigationMode.New:
                            ForwardStack.Clear();
                            if (prevEntry != null) {
                                if (BackStack.Count == CacheSize) {
                                    if (BackStack.Count > 0) {
                                        BackStack.RemoveAt(0);
                                    }
                                }

                                BackStack.Add(prevEntry);
                            }
                            break;

                        case NavigationMode.Back:
                            ForwardStack.Add(prevEntry);
                            BackStack.Remove(CurrentEntry);
                            break;

                        case NavigationMode.Forward:
                            BackStack.Add(prevEntry);
                            ForwardStack.Remove(CurrentEntry);
                            break;

                        case NavigationMode.Refresh:
                            break;
                    }
                }


                SourcePageType = entry.SourcePageType;

                Navigated?.Invoke(this, navEA);

                Dispatcher.UIThread.Post(() =>
                {
                    if (entry.Instance is Control newPage) {
                        navEA.RoutedEvent = NavigatedToEvent;
                        newPage.RaiseEvent(navEA);
                    }
                }, DispatcherPriority.Render);

                return true;
            }
            catch (Exception ex) {
                NavigationFailed?.Invoke(this, new NavigationFailedEventArgs(ex, entry.SourcePageType));
                return false;
            }
            finally {
                _isNavigating = false;
            }
        }

        private void OnNavigationStopped(PageStackEntry entry, NavigationMode mode) {
            NavigationStopped?.Invoke(this, new NavigationEventArgs(entry.Instance,
                mode, entry.NavigationTransitionInfo, entry.Parameter, entry.SourcePageType));
        }

        private void OnForwardStackChanged(object sender, NotifyCollectionChangedEventArgs e) {
            int oldCount = (_forwardStack.Count - (e.NewItems?.Count ?? 0) + (e.OldItems?.Count ?? 0));

            bool oldForward = oldCount > 0;
            bool newForward = _forwardStack.Count > 0;
            RaisePropertyChanged(CanGoForwardProperty, oldForward, newForward);
        }

        private void OnBackStackChanged(object sender, NotifyCollectionChangedEventArgs e) {
            int oldCount = (_backStack.Count - (e.NewItems?.Count ?? 0) + (e.OldItems?.Count ?? 0));

            bool oldBack = oldCount > 0;
            bool newBack = _backStack.Count > 0;
            RaisePropertyChanged(CanGoBackProperty, oldBack, newBack);
            RaisePropertyChanged(BackStackDepthProperty, oldCount, _backStack.Count);
        }

        private Control CreatePageAndCacheIfNecessary(Type srcPageType) {
            if (CacheSize == 0) {
                return NavigationPageFactory?.GetPage(srcPageType) ??
                    Activator.CreateInstance(srcPageType) as Control;
            }

            // This is triggered via Navigate(Type) - we only need to check the page type here
            for (int i = 0; i < _pageCache.Count; i++) {
                if (_pageCache[i].PageSrcType == srcPageType) {
                    throw new Exception($"An object of type {srcPageType} has already been added to the Navigation Stack");
                }
            }

            var newPage = NavigationPageFactory?.GetPage(srcPageType) ??
                Activator.CreateInstance(srcPageType) as Control;

            _pageCache.Add(new NavigationCacheItem(srcPageType, null, newPage));

            if (_pageCache.Count > CacheSize) {
                _pageCache.RemoveAt(0);
            }

            return newPage;
        }

        private Control CheckCacheAndGetPage(Type srcPageType = null, object target = null) {
            if (CacheSize == 0)
                return null;

            // v2 - Changes for cache
            // A page cached by Navigate(Type) and NavigateFromObject will be cached 
            // separately and not shared between the two - users should be consistent
            // here. Thus, srcType should be null when context isn't and vice-versa
            for (int i = _pageCache.Count - 1; i >= 0; i--) {
                var item = _pageCache[i];

                if (srcPageType != null && item.PageSrcType == srcPageType) {
                    // Call to Navigate(Type)
                    return item.Page;
                } else if (target != null && item.Context == target) {
                    // Call to NavigateFromObject()
                    return item.Page;
                }
            }

            return null;
        }

        private void TryAddToCache(object context, Control page) {
            // This is trigger by NavigateFromObject()

            // v2 - Changes for cache
            // A page cached by Navigate(Type) and NavigateFromObject will be cached 
            // separately and not shared between the two - users should be consistent
            // here. Thus, srcType should be null when context isn't and vice-versa
            for (int i = _pageCache.Count - 1; i >= 0; i--) {
                var item = _pageCache[i];
                if (context != null && item.Context == context) {
                    // Call to NavigateFromObject() - page is already cached
                    return;
                }
            }

            // Page is not cached - add it
            _pageCache.Add(new NavigationCacheItem(null, context, page));

            if (_pageCache.Count > CacheSize) {
                _pageCache.RemoveAt(0);
            }
        }

        private void SetContentAndAnimate(PageStackEntry entry) {
            if (entry == null)
                return;

            Content = entry.Instance;

            if (_presenter != null) {
                //Default to entrance transition
                entry.NavigationTransitionInfo = entry.NavigationTransitionInfo ?? new EntranceNavigationTransitionInfo();
                _presenter.Opacity = 0;

                _cts?.Cancel();
                _cts = new CancellationTokenSource();

                // Post the animation otherwise pages that take slightly longer to load won't
                // have an animation since it will run before layout is complete
                Dispatcher.UIThread.Post(() =>
                {
                    entry.NavigationTransitionInfo.RunAnimation(_presenter, _cts.Token);
                }, DispatcherPriority.Render);
            }
        }

        private void OnTopLevelBackRequested(object sender, RoutedEventArgs e) {
            if (!e.Handled && IsNavigationStackEnabled && CanGoBack) {
                GoBack();
                e.Handled = true;
            }
        }

        private CancellationTokenSource _cts;
        private ContentPresenter _presenter;
        private readonly List<NavigationCacheItem> _pageCache = new(10);
        private bool _isNavigating = false;

        private const string s_tpContentPresenter = "ContentPresenter";

        private class NavigationCacheItem {
            public NavigationCacheItem(Type pageType, object context, Control page) {
                if (pageType != null && context != null)
                    throw new InvalidOperationException("PageType and Context cannot both be set");

                PageSrcType = pageType;
                Context = context;
                Page = page;
            }

            public Type PageSrcType;
            public object Context;
            public Control Page;
        }
    }

    public class NavigationEventArgs : RoutedEventArgs {
        internal NavigationEventArgs(object content, NavigationMode mode,
            NavigationTransitionInfo navInfo, object param,
            Type srcPgType) {
            Content = content;
            NavigationMode = mode;
            NavigationTransitionInfo = navInfo;
            Parameter = param;
            SourcePageType = srcPgType;
        }

        public object Content { get; }

        public NavigationMode NavigationMode { get; }

        public object Parameter { get; }

        public Type SourcePageType { get; }

        public NavigationTransitionInfo NavigationTransitionInfo { get; }
    }

    public class PageStackEntry {
        public PageStackEntry(Type sourcePageType, object parameter, NavigationTransitionInfo navigationTransitionInfo) {
            NavigationTransitionInfo = navigationTransitionInfo;
            SourcePageType = sourcePageType;
            Parameter = parameter;
        }

        public NavigationTransitionInfo NavigationTransitionInfo { get; internal set; }

        public Type SourcePageType { get; set; }

        public object Parameter { get; set; }

        public object Context { get; internal set; }

        internal Control Instance { get; set; }
    }

    public class NavigationFailedEventArgs : EventArgs {
        internal NavigationFailedEventArgs(Exception ex, Type srcPageType) {
            Exception = ex;
            SourcePageType = srcPageType;
        }

        public bool Handled { get; set; }

        public Exception Exception { get; }

        public Type SourcePageType { get; }
    }

    public class NavigatingCancelEventArgs : RoutedEventArgs {
        internal NavigatingCancelEventArgs(NavigationMode mode, NavigationTransitionInfo info,
            object param, Type srcType) {
            NavigationMode = mode;
            NavigationTransitionInfo = info;
            Parameter = param;
            SourcePageType = srcType;
        }

        public bool Cancel { get; set; }

        public NavigationMode NavigationMode { get; }

        public Type SourcePageType { get; }

        public NavigationTransitionInfo NavigationTransitionInfo { get; }

        public object Parameter { get; }
    }

    public class FrameNavigationOptions {
        public NavigationTransitionInfo TransitionInfoOverride { get; set; }

        public bool IsNavigationStackEnabled { get; set; }
    }
}
