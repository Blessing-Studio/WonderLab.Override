using Avalonia.Controls.Documents;
using MinecraftLaunch.Modules.Enum;
using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Toolkits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.Class.Utils
{
    public class InlineUtils {
        public static InlineCollection CraftGameLogsInline(GameLogAnalyseResponse log) {
            var list = new InlineCollection();

            if (!(log.LogType is GameLogType.Exception) || !(log.LogType is GameLogType.StackTrace)) {
                list.Add(new Run("["));

                //time
                list.Add(new Run(log.Time) { 
                    Foreground = ThemeUtils.GetBrush("AccentBrush")
                });

                list.Add(new Run("]"));

                list.Add(new Run("["));

                //info
                list.Add(new Run(log.Source) {               
                    Foreground = ThemeUtils.GetBrush("AccentBrushDark2")
                });

                list.Add(new Run("/"));

                list.Add(new Run(log.LogType.ToString()) {               
                    Foreground = ThemeUtils.GetBrush("AccentBrushLight1")
                });

                list.Add(new Run("]"));
                list.Add(new Run(log.Log));
            }
            else {
            
            }

            return list;
        }
    }
}
