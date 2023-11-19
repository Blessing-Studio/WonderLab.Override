using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WonderLab.Classes.Interfaces {
    public interface IAbstractFactory<out T> where T : class {
        T Create();
    }
}
