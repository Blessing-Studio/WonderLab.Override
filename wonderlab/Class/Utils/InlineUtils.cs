using Avalonia.Controls.Documents;
using Avalonia.Media;
using DynamicData;
using MinecraftLaunch.Modules.Enum;
using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Toolkits;
using Newtonsoft.Json.Linq;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using wonderlab.Class.Enum;
using wonderlab.Class.Models;
using Color = Avalonia.Media.Color;

namespace wonderlab.Class.Utils {
    public class InlineUtils {
        private static readonly Dictionary<string, char> ColorNames = new() {
            { "black", '0' },
            { "darkblue", '1' },
            { "darkgreen", '2' },
            { "darkaqua", '3' },
            { "darkred", '4' },
            { "darkpurple", '5' },
            { "gold", '6' },
            { "gray", '7' },
            { "darkgray", '8' },
            { "blue", '9' },
            { "green", 'a' },
            { "aqua", 'b' },
            { "red", 'c' },
            { "light_purple", 'd' },
            { "yellow", 'e' },
            { "white", 'f' },
            { "minecoingold", 'g' },
        };

        private static readonly Dictionary<char, Color> Colors = new() {
            { '0', MotdColor.Black },
            { '1', MotdColor.DarkBlue },                             
            { '2', MotdColor.DarkGreen },          
            { '3', MotdColor.DarkAqua },
            { '4', MotdColor.DarkRed },
            { '5', MotdColor.DarkPurple },
            { '6', MotdColor.Gold },
            { '7', MotdColor.Gray },
            { '8', MotdColor.DarkGray },
            { '9', MotdColor.Blue },
            { 'a', MotdColor.Green },
            { 'b', MotdColor.Aqua },
            { 'c', MotdColor.Red },
            { 'd', MotdColor.LightPurple },
            { 'e', MotdColor.Yellow },
            { 'f', MotdColor.White },                  
            { 'g', MotdColor.MineCoinGold }                             
        };

        private static readonly Dictionary<char, MotdTextFormat> Formats = new() {       
            { 'k', MotdTextFormat.Obfuscated },
            { 'l', MotdTextFormat.Bold },
            { 'm', MotdTextFormat.StrikeThrough },                
            { 'n', MotdTextFormat.Underline },                   
            { 'o', MotdTextFormat.Italic },            
            { 'r',  MotdTextFormat.Reset }                 
        };

        public static string GetColorCode(string colorName) {
            char result = 'f';

            if (ColorNames.ContainsKey(colorName)) {
                ColorNames.TryGetValue(colorName, out result);
                return result.ToString();
            }

            return $"§{result}" ?? "f";
        }

        public static string GetFormat(JToken info) {
            if (info == null) {
                return string.Empty;
            }

            if (!info["bold"].IsNull() && info["bold"].Type is JTokenType.Boolean) {
                var isBold = Convert.ToBoolean(info["bold"].ToString());
                return isBold ? $"§l" : string.Empty;
            }

            return string.Empty;
        }

        public static InlineCollection CraftGameLogsInline(GameLogAnalyseResponse log) {
            var list = new InlineCollection();

            if (!(log.LogType is GameLogType.Exception) && !(log.LogType is GameLogType.Unknown) && !(log.LogType is GameLogType.StackTrace)) {
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

                if(!string.IsNullOrEmpty(log.Source)) {
                    list.Add(new Run("/"));
                }

                list.Add(new Run(log.LogType.ToString()) {               
                    Foreground = ThemeUtils.GetBrush("AccentBrushLight1")
                });

                list.Add(new Run("]"));
                list.Add(new Run(log.Log));
            }
            else if (log.LogType is GameLogType.Unknown) {
                list.Add(new Run(log.Log));
            }
            else if(log.LogType is GameLogType.Exception || log.LogType is GameLogType.StackTrace) {         
                list.Add(new Run(log.Log) {
                    Foreground = new SolidColorBrush(Color.Parse("#FD475D"))
                });;
            }

            return list;
        }

        public static InlineCollection CraftServerMotdInline(string motd) {
            var list = new InlineCollection();

            Color lastColor = MotdColor.White;
            MotdTextFormat lastTextFormat = MotdTextFormat.Reset;
            motd = motd.Trim().Replace('\n', ' ');
            List<char> chars = new();
            int num = 0;
            char[] array = motd.ToCharArray();

            foreach (char c in array) {
                if (c == ' ' && num <= 2) {
                    chars.Add(c);
                    num++;
                } else if (c != ' ') {
                    chars.Add(c);
                    num = 0;
                }
            }

            bool symbolFlag = false;
            for (int i = 0; i < chars.Count; i++) {
                if (symbolFlag) {
                    symbolFlag = false;
                    continue;
                }

                if (i != chars.Count - 1 && chars[i] == '§' && (Colors.ContainsKey(chars[i + 1]) || Formats.ContainsKey(chars[i + 1]))) {
                    char key = chars[i + 1];
                    if (Colors.ContainsKey(key)) {
                        lastColor = Colors[key];
                    }

                    if (Formats.ContainsKey(key)) {
                        lastTextFormat = Formats[key];
                    }

                    symbolFlag = true;
                    continue;
                }

                Run run = new Run(chars[i].ToString());
                if (lastColor == MotdColor.White) {
                    run.Foreground = Brushes.Black;
                } else {
                    run.Foreground = new SolidColorBrush(lastColor);
                }

                switch (lastTextFormat) {
                    case MotdTextFormat.Bold:
                        run.FontWeight = FontWeight.Bold;
                        break;
                    case MotdTextFormat.Italic:
                        run.FontStyle = FontStyle.Italic;
                        break;
                    case MotdTextFormat.Obfuscated:
                        run.FontStyle = FontStyle.Oblique;
                        break;
                    case MotdTextFormat.StrikeThrough:
                        run.TextDecorations.Add(TextDecorations.Strikethrough);
                        break;
                    case MotdTextFormat.Underline:
                        run.TextDecorations.Add(TextDecorations.Underline);
                        break;
                    case MotdTextFormat.Reset:
                        run.TextDecorations?.Clear();
                        run.FontWeight = FontWeight.Regular;
                        run.FontStyle = FontStyle.Normal;
                        break;
                }
                list.Add(run);
            }

            return list;
        }
    }
}