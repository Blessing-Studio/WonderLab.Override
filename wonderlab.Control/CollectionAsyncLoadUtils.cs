using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.control {
    public class CollectionAsyncLoadUtils<T> {
        private int count;

        private bool bPause;

        private ObservableCollection<T> _sourceData = null!, _targetData = null!;

        public event Action Loaded;

        public void Pause() {
            bPause = true;
        }

        public async void Continue() {
            bPause = false;
            await AddItem();
        }

        public async void Load(ObservableCollection<T> targetData, IEnumerable<T> sourceData) {
            if (sourceData == null || targetData == null) {
                return;
            }

            _sourceData = new ObservableCollection<T>(sourceData);
            _targetData = targetData;
            await LoadAsync();
        }

        private async ValueTask LoadAsync() {
            count = 0;
            await AddItem();
        }

        private async ValueTask AddItem() {
            if (bPause) {
                return;
            }

            if (count < _sourceData.Count) {
                await Task.Delay(5);
                _targetData.Add(_sourceData[count++]);
                await AddItem();
            } else {
                _sourceData = null;
                Loaded?.Invoke();
            }
        }
    }

    public static class ObservableCollectionExtensions {
        public static CollectionAsyncLoadUtils<T> Load<T>(this ObservableCollection<T> targetData, IEnumerable<T> sourceData) {
            var helper = new CollectionAsyncLoadUtils<T>();
            helper.Load(targetData, sourceData);
            return helper;
        }
    }
}
