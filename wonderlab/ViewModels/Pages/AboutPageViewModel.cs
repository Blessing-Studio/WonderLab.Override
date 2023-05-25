using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Utils;
using wonderlab.Views.Pages;

namespace wonderlab.ViewModels.Pages
{
    public class AboutPageViewModel : ReactiveObject {   
        public void GoBackAction() {
            new SelectConfigPage().Navigation();
        }
    }
}
