using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using wonderlab.control.Controls.Dialog;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace wonderlab.control.Controls {
    public class PageSwitcher : TemplatedControl {
        private ListBox ListBox = null!;

        private TextBlock PageNumberDisplay = null!;

        private Dictionary<int, IEnumerable<object>> Cache = null!;

        private int Total = 0;

        private Button BackButton, NextPageButton;

        public IEnumerable Items { get => GetValue(ItemsProperty); set => SetValue(ItemsProperty, value); }

        public int CurrentMaxItemsCount { get => GetValue(CurrentMaxItemsCountProperty); set => SetValue(CurrentMaxItemsCountProperty, value); }

        public int CurrentItemsIndex { get => GetValue(CurrentItemsIndexProperty); set => SetValue(CurrentItemsIndexProperty, value); }

        public static readonly StyledProperty<IEnumerable> ItemsProperty =
            AvaloniaProperty.Register<PageSwitcher, IEnumerable>(nameof(Items));

        public static readonly StyledProperty<int> CurrentMaxItemsCountProperty =
            AvaloniaProperty.Register<PageSwitcher, int>(nameof(CurrentMaxItemsCount), 15);

        public static readonly StyledProperty<int> CurrentItemsIndexProperty =
            AvaloniaProperty.Register<PageSwitcher, int>(nameof(CurrentItemsIndex), 1);

        public PageSwitcher() {
        }

        private string GetPageNumberText() {
            return $"{CurrentItemsIndex} / {Total}";
        }

        private void GetTotalPageNumber() {
            List<object> list = new();
            var enumerator = Items.GetEnumerator();

            while (enumerator.MoveNext()) {
                list.Add(enumerator.Current);
            }

            //此处使用 decimal 类型进行计算，目的是获取小数进行四舍五入运算
            decimal total = Convert.ToDecimal(list.Count) / Convert.ToDecimal(CurrentMaxItemsCount);

            if (total - (int)total > 0) {
                Total = (int)total + 1;
            }

            Total = (int)total;
        }

        private void SplitListToDictionary() {
            List<object> list = new();
            Dictionary<int, IEnumerable<object>> result = new();

            var enumerator = Items.GetEnumerator();
            while (enumerator.MoveNext()) {
                list.Add(enumerator.Current);
            }

            for (int i = 0; i < Total; i++) {
                var block = list.Skip(i * CurrentMaxItemsCount)
                                .Take(CurrentMaxItemsCount);
                result.Add(i + 1, block);
            }

            Cache = result;
        }

        private void GoNextPage(object? sender, RoutedEventArgs e) {
            if (CurrentItemsIndex + 1 <= Cache.Keys.LastOrDefault()) {
                CurrentItemsIndex++;
                BackButton.IsEnabled = true;
            } else {
                NextPageButton.IsEnabled = false;
            }

            if (Cache != null && Cache.Count > 0 && Cache.ContainsKey(CurrentItemsIndex)) {
                var cache = new ObservableCollection<object>();                
                ListBox.ItemsSource = cache;
                cache.Load(Cache[CurrentItemsIndex]);

                PageNumberDisplay.Text = GetPageNumberText();
            }

            if (!Cache.ContainsKey(CurrentItemsIndex + 1)) {
                NextPageButton.IsEnabled = false;
            }
        }

        private void GoBack(object? sender, RoutedEventArgs e) {
            if (CurrentItemsIndex - 1 > 0) {
                CurrentItemsIndex--;
                NextPageButton.IsEnabled = true;
            }

            if (Cache != null && Cache.Any() && Cache.ContainsKey(CurrentItemsIndex)) {
                var cache = new ObservableCollection<object>();
                ListBox.ItemsSource = cache;
                cache.Load(Cache[CurrentItemsIndex]);
                PageNumberDisplay.Text = GetPageNumberText();
            }

            if (!Cache.ContainsKey(CurrentItemsIndex - 1)) {
                BackButton.IsEnabled = false;
            }
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
            base.OnApplyTemplate(e);

            ListBox = e.NameScope.Find<ListBox>("ItemsList")!;
            PageNumberDisplay = e.NameScope.Find<TextBlock>("display")!;
            BackButton = e.NameScope.Find<Button>("BackButton")!;
            BackButton!.Click += GoBack;

            NextPageButton = e.NameScope.Find<Button>("NextPageButton")!;
            NextPageButton!.Click += GoNextPage;
            BackButton.IsEnabled = false;
            ListBox.Items.CollectionChanged += (_, _) => {
                if (!Cache.ContainsKey(CurrentItemsIndex - 1)) {
                    BackButton.IsEnabled = false;
                }

                if (!Cache.ContainsKey(CurrentItemsIndex + 1)) {
                    NextPageButton.IsEnabled = false;
                }

                if (CurrentItemsIndex == 1 && Cache.Keys.Any() && !NextPageButton.IsEnabled) {
                    NextPageButton.IsEnabled = true;
                }
            };
        }

        protected override async void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
            base.OnPropertyChanged(change);

            if (change.Property == ItemsProperty) {
                CurrentItemsIndex = 1;
                if (ListBox is null && PageNumberDisplay is null) {
                    await Task.Delay(1000);
                }

                GetTotalPageNumber();
                SplitListToDictionary();

                if (ListBox is not null && PageNumberDisplay is not null && Cache.Count > 0) {
                    var cache = new ObservableCollection<object>();
                    ListBox.ItemsSource = cache;
                    cache.Load(Cache[CurrentItemsIndex]);
                    PageNumberDisplay.Text = GetPageNumberText();
                }
            }
        }
    }
}
