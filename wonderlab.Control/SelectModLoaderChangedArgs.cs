using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.control {
    public class SelectModLoaderChangedArgs : EventArgs {
        public string ModLoaderName { get; set; }

        public static SelectModLoaderChangedArgs Build(string name) {
            return new SelectModLoaderChangedArgs {
                ModLoaderName = name
            };
        }
    }
}
