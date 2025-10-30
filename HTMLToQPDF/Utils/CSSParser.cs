using System.Text.RegularExpressions;

namespace HTMLToQPDF.Utils
{
    internal static class CSSParser
    {
        /// <summary>
        /// Parses an inline CSS style attribute into a dictionary of property-value pairs.
        /// Example: "color: red; font-weight: bold;" => { "color": "red", "font-weight": "bold" }
        /// </summary>
        public static Dictionary<string, string> ParseInlineStyle(string? styleAttribute)
        {
            var styles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            
            if (string.IsNullOrWhiteSpace(styleAttribute))
                return styles;

            // Split by semicolon, but be careful with functions like rgb()
            var declarations = SplitStyleDeclarations(styleAttribute);

            foreach (var declaration in declarations)
            {
                var colonIndex = declaration.IndexOf(':');
                if (colonIndex <= 0) continue;

                var property = declaration.Substring(0, colonIndex).Trim();
                var value = declaration.Substring(colonIndex + 1).Trim();

                if (!string.IsNullOrEmpty(property) && !string.IsNullOrEmpty(value))
                {
                    styles[property] = value;
                }
            }

            return styles;
        }

        /// <summary>
        /// Splits style declarations by semicolon, handling nested parentheses (e.g., rgb() functions)
        /// </summary>
        private static List<string> SplitStyleDeclarations(string styleAttribute)
        {
            var declarations = new List<string>();
            var currentDeclaration = "";
            var parenDepth = 0;

            foreach (var ch in styleAttribute)
            {
                if (ch == '(')
                {
                    parenDepth++;
                    currentDeclaration += ch;
                }
                else if (ch == ')')
                {
                    parenDepth--;
                    currentDeclaration += ch;
                }
                else if (ch == ';' && parenDepth == 0)
                {
                    if (!string.IsNullOrWhiteSpace(currentDeclaration))
                    {
                        declarations.Add(currentDeclaration);
                    }
                    currentDeclaration = "";
                }
                else
                {
                    currentDeclaration += ch;
                }
            }

            // Add the last declaration if there's no trailing semicolon
            if (!string.IsNullOrWhiteSpace(currentDeclaration))
            {
                declarations.Add(currentDeclaration);
            }

            return declarations;
        }

        public static string? ParseColor(string colorValue)
        {
            if (string.IsNullOrWhiteSpace(colorValue))
                return null;

            colorValue = colorValue.Trim();

            // Handle rgb() or rgba()
            var rgbMatch = Regex.Match(colorValue, @"rgba?\s*\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*(?:,\s*[\d.]+\s*)?\)", RegexOptions.IgnoreCase);
            if (rgbMatch.Success)
            {
                var r = byte.Parse(rgbMatch.Groups[1].Value);
                var g = byte.Parse(rgbMatch.Groups[2].Value);
                var b = byte.Parse(rgbMatch.Groups[3].Value);
                return $"#{r:X2}{g:X2}{b:X2}";
            }

            // Handle hex colors
            if (colorValue.StartsWith("#"))
            {
                return colorValue;
            }

            // Handle named colors - convert common ones to hex
            return ConvertNamedColor(colorValue);
        }

        private static string? ConvertNamedColor(string colorName)
        {
            var namedColors = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "black",      "#000000" },
                { "white",      "#FFFFFF" },
                { "red",        "#FF0000" },
                { "green",      "#008000" },
                { "blue",       "#0000FF" },
                { "yellow",     "#FFFF00" },
                { "cyan",       "#00FFFF" },
                { "magenta",    "#FF00FF" },
                { "gray",       "#808080" },
                { "grey",       "#808080" },
                { "silver",     "#C0C0C0" },
                { "maroon",     "#800000" },
                { "olive",      "#808000" },
                { "lime",       "#00FF00" },
                { "aqua",       "#00FFFF" },
                { "teal",       "#008080" },
                { "navy",       "#000080" },
                { "fuchsia",    "#FF00FF" },
                { "purple",     "#800080" },
                { "orange",     "#FFA500" }
            };

            return namedColors.TryGetValue(colorName, out var hexColor) ? hexColor : null;
        }
    }
}
