using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WonderLab.Classes.Datas;
 
public sealed record NavigationPageData {
    public required object Page {  get; init; }

    public required string PageKey { get; init; }
}
