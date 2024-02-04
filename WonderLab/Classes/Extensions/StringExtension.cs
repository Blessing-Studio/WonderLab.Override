using System;

namespace WonderLab.Classes.Extensions;

public static class StringExtension
{
    public static int ToInt(this string text)
    {
        return Convert.ToInt32(text);
    }
}
