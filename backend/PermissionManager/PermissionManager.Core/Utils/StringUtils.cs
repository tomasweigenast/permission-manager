using System.Text;

namespace PermissionManager.Core.Utils;

public static class StringUtils
{
    /// <summary>
    /// Converts <see cref="input"/> to snake_case
    /// </summary>
    public static string ToSnakeCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var builder = new StringBuilder();
        for (var i = 0; i < input.Length; i++)
        {
            var c = input[i];
            if (char.IsUpper(c))
            {
                if (i > 0)
                    builder.Append('_');
                builder.Append(char.ToLower(c));
            }
            else
                builder.Append(c);
        }
        
        return builder.ToString();
    }
}