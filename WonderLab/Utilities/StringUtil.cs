using System.Text.RegularExpressions;

namespace WonderLab.Utilities;
public sealed partial class StringUtil {
    [GeneratedRegex(@"(?<=^.{5}).*(?=.{5}$)")]
    private static partial Regex ReplaceUuidRegex();

    public static string ReplaceUuid(string input) {
        return ReplaceUuidRegex().Replace(input, m => new string('*', m.Length));
    }
}