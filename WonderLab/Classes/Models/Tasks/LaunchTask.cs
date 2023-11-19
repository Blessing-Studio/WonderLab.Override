using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WonderLab.Classes.Models.Tasks {
    public class LaunchTask : TaskBase {
        public override ValueTask BuildWorkItemAsync(CancellationToken token) {
            throw new NotImplementedException();    
        }
    }
}
