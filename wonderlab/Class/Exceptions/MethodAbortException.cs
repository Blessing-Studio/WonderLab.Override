using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.Class.Exceptions {
    public class MethodAbortException : Exception {
        public bool IsArtificial { get; }

        public MethodAbortException(string message, bool isArtificial) : base(message) {
            IsArtificial = isArtificial;
        }
    }
}
