using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.Class.ViewData
{
    public class ViewDataBase<T> : ReactiveObject {
        [Reactive]
        public T Data { get; set; }

        public ViewDataBase(T data) : base() {       
            this.Data = data;
        }
    }
}
