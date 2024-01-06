using System;
using System.Linq;
using System.Collections.Generic;
using WonderLab.Classes.Models.ViewData;

namespace WonderLab.Extensions {
    public static class ViewDataExtension {
        public static TResult CreateViewData<TData, TResult>(this TData data) where TResult : ViewDataBase<TData>
            => (Activator.CreateInstance(typeof(TResult), data!)! as TResult)!;

        public static IEnumerable<TResult> CreateEnumerable<TData, TResult>(this IEnumerable<TData> data) where TResult : ViewDataBase<TData>
            => data.Select(x => x.CreateViewData<TData, TResult>());
    }
}
