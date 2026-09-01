using System.Collections.Generic;
using System.Globalization;
public static class ValueExtractorr
{
    public static List<decimal> Extract(string[] parts)
    {
        List<decimal> values = new List<decimal>();

        for (int i = 14; i < parts.Length; i++)
        {
            if (decimal.TryParse(parts[i], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal v))
                values.Add(v);
            else if (parts[i] == "A"
                && i + 1 < parts.Length
                && decimal.TryParse(parts[i + 1], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal av))
            {
                values.Add(av);
                i++;
            }
        }
        return values;
    }
}
