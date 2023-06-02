using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using wonderlab.Class.Models;
using System.Text.RegularExpressions;

namespace wonderlab.Class.Utils {
    public static class SystemUtils {
        private static readonly PerformanceCounter FreeMemoryCounter = new("Memory", "Available MBytes");

        private static readonly PerformanceCounter MemoryUsagePercentageCounter = new("Memory", "% Committed Bytes In Use");

        public static bool IsMacOS => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        public static bool IsWindows11 => (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && Environment.OSVersion.Version.Build >= 22000);

        public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        public static string GetPlatformName() {
            if (IsWindows) {
                return "Windows";
            } else if (IsLinux) {
                return "Linux";
            } else {
                return "MacOS";
            }
        }

        public static MemoryInfo GetMemoryInfo() {
            if (IsWindows) {
                return GetWindowsMemoryInfo();
            } else if (IsLinux) {
                return GetLinuxMemoryInfo();
            } else {
                return GetMacMemoryInfo();
            }
        }

        private static MemoryInfo GetLinuxMemoryInfo() {
            var info = new ProcessStartInfo {
                FileName = "/bin/bash",
                Arguments = "-c \"free -m\"",
                RedirectStandardOutput = true
            };

            using var process = Process.Start(info);

            if (process == null)
                return new MemoryInfo {
                    Total = -1,
                    Used = -1,
                    Free = -1,
                    Percentage = -1
                };

            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            // Console.WriteLine(output);

            var lines = output.Split('\n');
            var memory = lines[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);

            var total = double.Parse(memory[1]);
            var used = double.Parse(memory[2]);
            var free = double.Parse(memory[3]);
            var percentage = used / total;

            var metrics = new MemoryInfo {
                Total = total,
                Used = used,
                Free = free,
                Percentage = percentage * 100
            };

            return metrics;
        }

        private static MemoryInfo GetWindowsMemoryInfo() {
            var free = FreeMemoryCounter.NextValue();
            var total = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / Math.Pow(1024, 2);
            var used = total - free;
            var percentage = MemoryUsagePercentageCounter.NextValue();

            var result = new MemoryInfo {
                Free = free,
                Percentage = percentage,
                Total = total,
                Used = used
            };

            return result;
        }

        private static MemoryInfo GetMacMemoryInfo() {
            var info = new ProcessStartInfo {
                FileName = "/bin/bash",
                Arguments = $"-c \"vm_stat\"",
                RedirectStandardOutput = true
            };
            using var process = Process.Start(info);

            if (process == null) {
                return new MemoryInfo {
                    Total = -1,
                    Used = -1,
                    Free = -1,
                    Percentage = -1
                };
            }

            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            if (string.IsNullOrEmpty(output)) {
                return new MemoryInfo {
                    Total = -1,
                    Used = -1,
                    Free = -1,
                    Percentage = -1
                };
            }

            var split = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var infoDic = split
                .Skip(1)
                .Select(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                .Select(lineSplit => (lineSplit.Take(lineSplit.Length - 1), lineSplit.Last().TrimEnd('.')))
                .Select(pair => (string.Join(' ', pair.Item1).TrimEnd(':'), double.TryParse(pair.Item2, out var outVal) ? outVal : 0))
                .ToDictionary(pair => pair.Item1, pair2 => pair2.Item2);

            var pageSize = uint.TryParse(Regex.Match(split[0], "\\d+").Value, out var pageSizeOut) ? pageSizeOut : 0;
            var active = (infoDic.TryGetValue("Pages active", out var activeOut) ? activeOut : 0) * pageSize;

            var used = active / Math.Pow(1024, 2);
            var total = GetTotalMemory() / Math.Pow(1024, 2);
            var free = total - used;
            var percentage = used / total;

            var metrics = new MemoryInfo {
                Total = total,
                Used = used,
                Free = free,
                Percentage = percentage * 100
            };

            return metrics;

            static ulong GetTotalMemory() {
                var info = new ProcessStartInfo {
                    FileName = "/usr/sbin/sysctl",
                    Arguments = "hw.memsize",
                    RedirectStandardOutput = true
                };

                using var process = Process.Start(info);
                if (process == null) {
                    return 0;
                }

                var output = process.StandardOutput?.ReadToEnd();
                process.WaitForExit();
                if (string.IsNullOrEmpty(output)) {
                    return 0;
                }

                var split = output.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var value = split.Last();

                return ulong.TryParse(value, out var outVal) ? outVal : 0;
            }
        }
    }
}
