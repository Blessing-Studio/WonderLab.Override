using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.Class.ViewData
{
    public class ModLoaderViewData<T> : ViewDataBase<T> { 
        public ModLoaderViewData(T data) : base(data) {       
        }
    }
}
