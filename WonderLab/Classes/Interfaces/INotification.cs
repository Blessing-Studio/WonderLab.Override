using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WonderLab.Classes.Interfaces {
    public interface INotification {
        public string Header { get; set; }
        public string Message { get; set; }
    }
}
