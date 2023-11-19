using Avalonia;
using Avalonia.Styling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WonderLab.Classes.Managers
{
    public class ThemeManager
    {
        private Application _application = Application.Current!;

        public void Change(ThemeVariant variant)
        {
            _application!.RequestedThemeVariant = variant;
        }
    }
}
