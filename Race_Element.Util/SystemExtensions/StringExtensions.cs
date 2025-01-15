using System.Text;

namespace RaceElement.Util.SystemExtensions;

public static class StringExtensions
{
    public static string FillStart(this string value, int maxLength, char filler)
    {
        if (string.IsNullOrEmpty(value)) return new string(filler, maxLength);
        return value.PadLeft(maxLength, filler);
    }

    public static string FillEnd(this string value, int maxLength, char filler)
    {
        if (string.IsNullOrEmpty(value)) return new string(filler, maxLength);
        return value.PadRight(maxLength, filler);
    }

    public static string ToString(this string[] values)
    {
        if (values == null || values.Length == 0) return string.Empty;

        var builder = new StringBuilder();
        for (int i = 0; i < values.Length; i++)
        {
            string v = values[i];
            builder.Append($"{{{v}}}");
            if (i < values.Length - 1)
                builder.Append(", ");
        }
        return builder.ToString();
    }
}
