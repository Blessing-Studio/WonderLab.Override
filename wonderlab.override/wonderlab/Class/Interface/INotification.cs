using Avalonia.Controls.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.Class.Interface
{
    public interface INotification
    {
        string Title { set; get; }

        string RunTime { set; get; }

        void Begin();
    }
}
