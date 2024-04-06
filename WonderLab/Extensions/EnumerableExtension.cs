using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WonderLab.Extensions;

public static class EnumerableExtension {
    public static ObservableCollection<T> ToObservableList<T>(this IEnumerable<T> values) {
        return new(values);
    }
}