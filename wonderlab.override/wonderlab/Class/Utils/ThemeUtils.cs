using Avalonia.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.Class.Utils
{
    public class ThemeUtils {
        public void SetAccentColor(object key, object value) {
            if (AccentColorResources.ContainsKey(key)) {
                AccentColorResources[key] = value;
            }
            else {
                AccentColorResources.Add(key, value);
            }

        }

        public static readonly ResourceDictionary AccentColorResources = new ResourceDictionary();
    }
}
