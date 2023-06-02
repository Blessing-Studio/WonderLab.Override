using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.Class.Models {
    public class MemoryInfo {
        public double Used { get; set; }

        public double Free { get; set; }

        public double Total { get; set; }

        public double Percentage { get; set; }

        public override string ToString() {
            return $"内存百分比 {Percentage}%";
        }
    }
}
