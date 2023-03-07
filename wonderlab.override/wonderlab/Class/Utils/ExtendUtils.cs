using Avalonia.Data.Core;
using MinecraftLaunch.Modules.Models.Launch;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.ViewData;

namespace wonderlab.Class.Utils
{
    public static class ExtendUtils
    {
        public static void ShowMessage(this string message) {
            MainWindow.Instance?.ShowInfoBar("信息", message);
        }

        public static double ToDouble(this object obj) { 
            return Convert.ToDouble(obj);
        }

        public static IEnumerable<T> ToEnumerable<T>(this IAsyncEnumerable<T> obj) {
            var enumerator = obj.GetAsyncEnumerator();

            while (enumerator.MoveNextAsync().Result) {           
                yield return enumerator.Current;
            }
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> obj) {             
            return new(obj.Select(x => x).Distinct());
        }

        public static TResult CreateViewData<TData, TResult>(this TData data) where TResult : ViewDataBase<TData>
              => Activator.CreateInstance(typeof(TResult), data)! as TResult;

        public static bool CanUpdate(this UpdateInfo info) {
            if (string.IsNullOrEmpty(info.Title)) {
                return false;
            }

            if(info.Title.Split('-').First() is not UpdateUtils.VersionType) { 
                return false;
            }

            var intVersion = Convert.ToInt32(info.Title.Split('-').Last().Replace(".", string.Empty));
            return intVersion > UpdateUtils.Version;
        }
    }
}
