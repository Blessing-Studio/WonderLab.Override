using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WonderLab.Classes.Interfaces.Navigation {
    public interface INavigationPageFactory {
        Control GetPage(Type srcType);

        Control GetPageFromObject(object target);
    }
}
