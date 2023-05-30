﻿using Avalonia;
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

        public IEnumerable Items { get => GetValue(ItemsProperty); set => SetValue(ItemsProperty, value); }

        public int CurrentMaxItemsCount { get => GetValue(CurrentMaxItemsCountProperty); set => SetValue(CurrentMaxItemsCountProperty, value); }

        public int CurrentItemsIndex { get => GetValue(CurrentItemsIndexProperty); set => SetValue(CurrentItemsIndexProperty, value); }

        //Property
        public static readonly StyledProperty<IEnumerable> ItemsProperty =
            AvaloniaProperty.Register<PageSwitcher, IEnumerable>(nameof(Items));

        public static readonly StyledProperty<int> CurrentMaxItemsCountProperty =
            AvaloniaProperty.Register<PageSwitcher, int>(nameof(CurrentMaxItemsCount), 15);

        public static readonly StyledProperty<int> CurrentItemsIndexProperty =
            AvaloniaProperty.Register<PageSwitcher, int>(nameof(CurrentItemsIndex), 1);

        public PageSwitcher() {
        }

        public PageSwitcher(IEnumerable items) {
            Items = items;
            GetTotalPageNumber();
            SplitListToDictionary();
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
            decimal total = Convert.ToDecimal(list.Count()) / Convert.ToDecimal(CurrentMaxItemsCount);

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

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
            base.OnApplyTemplate(e);
            
            ListBox = e.NameScope.Find<ListBox>("ItemsList")!;
            PageNumberDisplay = e.NameScope.Find<TextBlock>("display")!;
            e.NameScope.Find<Button>("BackButton")!.Click += GoBack;
            e.NameScope.Find<Button>("NextPageButton")!.Click += GoNextPage;

            //Dispatcher.UIThread.Post(() => {
            //    ListBox.Items = Cache[CurrentItemsIndex];
            //    PageNumberDisplay.Text = GetPageNumberText();
            //});
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

                ListBox.Items = Cache[CurrentItemsIndex];
                PageNumberDisplay.Text = GetPageNumberText();
            }
        }

        private void GoNextPage(object? sender, RoutedEventArgs e) {
            CurrentItemsIndex++;

            if (Cache != null && Cache.Count > 0 && Cache.ContainsKey(CurrentItemsIndex)) {
                ListBox.Items = Cache[CurrentItemsIndex];
                PageNumberDisplay.Text = GetPageNumberText();
            }
        }

        private void GoBack(object? sender, RoutedEventArgs e) {
            CurrentItemsIndex--;
            if (Cache != null && Cache.Count > 0 && Cache.ContainsKey(CurrentItemsIndex)) {
                ListBox.Items = Cache[CurrentItemsIndex];
                PageNumberDisplay.Text = GetPageNumberText();
            }
        }
    }
}
