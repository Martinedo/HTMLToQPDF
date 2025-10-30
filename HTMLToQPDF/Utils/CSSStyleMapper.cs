using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace HTMLToQPDF.Utils
{
    internal static class CSSStyleMapper
    {
        public static TextStyle ApplyInlineStyles(TextStyle baseStyle, Dictionary<string, string> cssProperties)
        {
            var style = baseStyle;

            foreach (var (property, value) in cssProperties)
            {
                style = ApplyProperty(style, property, value);
            }

            return style;
        }

        private static TextStyle ApplyProperty(TextStyle style, string property, string value)
        {
            return property.ToLower() switch
            {
                "color" => ApplyColor(style, value),
                "background-color" or "background" => ApplyBackgroundColor(style, value),
                "font-weight" => ApplyFontWeight(style, value),
                "font-style" => ApplyFontStyle(style, value),
                "font-family" => ApplyFontFamily(style, value),
                "font-size" => ApplyFontSize(style, value),
                "text-decoration" or "text-decoration-line" => ApplyTextDecoration(style, value),
                "line-height" => ApplyLineHeight(style, value),
                "letter-spacing" => ApplyLetterSpacing(style, value),
                "word-spacing" => ApplyWordSpacing(style, value),
                _ => style // Ignore unsupported properties
            };
        }

        private static TextStyle ApplyColor(TextStyle style, string value)
        {
            var color = CSSParser.ParseColor(value);
            if (color != null)
            {
                style = style.FontColor(color);
            }
            return style;
        }

        private static TextStyle ApplyBackgroundColor(TextStyle style, string value)
        {
            var color = CSSParser.ParseColor(value);
            if (color != null)
            {
                style = style.BackgroundColor(color);
            }
            return style;
        }

        private static TextStyle ApplyFontWeight(TextStyle style, string value)
        {
            value = value.ToLower().Trim();
            
            // Handle numeric values (100-900)
            if (int.TryParse(value, out int weight))
            {
                if (weight >= 600) // 600+ is considered bold
                {
                    style = style.Bold();
                }
                // Normal weight (< 600) - keep as is
            }
            // Handle keyword values
            else if (value == "bold" || value == "bolder")
            {
                style = style.Bold();
            }
            // "normal" or "lighter" - keep as is
            
            return style;
        }

        private static TextStyle ApplyFontStyle(TextStyle style, string value)
        {
            value = value.ToLower().Trim();
            
            if (value == "italic" || value == "oblique")
            {
                style = style.Italic();
            }
            
            return style;
        }

        private static TextStyle ApplyFontFamily(TextStyle style, string value)
        {
            // Remove quotes and trim
            value = value.Trim().Trim('\'', '"');
            
            if (!string.IsNullOrEmpty(value))
            {
                // Take the first font family if multiple are specified
                var firstFont = value.Split(',')[0].Trim().Trim('\'', '"');
                style = style.FontFamily(firstFont);
            }
            
            return style;
        }

        private static TextStyle ApplyFontSize(TextStyle style, string value)
        {
            value = value.ToLower().Trim();
            
            // Handle CSS font-size keywords
            var keywordSize = value switch
            {
                "xx-small" => 9f,
                "x-small" => 10f,
                "small" => 13f,
                "medium" => 16f,
                "large" => 18f,
                "x-large" => 24f,
                "xx-large" => 32f,
                "xxx-large" => 48f,
                "smaller" => -2f, // Relative, will reduce by 2pt
                "larger" => 2f,   // Relative, will increase by 2pt
                _ => 0f
            };
            
            if (keywordSize != 0f)
            {
                style = style.FontSize(keywordSize);
                return style;
            }
            
            if (value.EndsWith("px"))
            {
                var sizeStr = value.Substring(0, value.Length - 2);
                if (float.TryParse(sizeStr, out float size))
                {
                    style = style.FontSize(size);
                }
            }
            else if (value.EndsWith("pt"))
            {
                var sizeStr = value.Substring(0, value.Length - 2);
                if (float.TryParse(sizeStr, out float size))
                {
                    style = style.FontSize(size);
                }
            }
            // Handle unitless numbers (treated as px)
            else if (float.TryParse(value, out float size))
            {
                style = style.FontSize(size);
            }
            
            return style;
        }

        private static TextStyle ApplyTextDecoration(TextStyle style, string value)
        {
            value = value.ToLower().Trim();
            
            if (value.Contains("underline"))
            {
                style = style.Underline();
            }
            
            if (value.Contains("line-through"))
            {
                style = style.Strikethrough();
            }
            
            return style;
        }

        private static TextStyle ApplyLineHeight(TextStyle style, string value)
        {
            value = value.ToLower().Trim();
            
            // Handle numeric multiplier (e.g., "1.5")
            if (float.TryParse(value, out float multiplier))
            {
                style = style.LineHeight(multiplier);
            }
            else if (value.EndsWith("px"))
            {
                var sizeStr = value.Substring(0, value.Length - 2);
                if (float.TryParse(sizeStr, out float size))
                {
                    // Convert to multiplier (approximation)
                    style = style.LineHeight(size / 16); // Assuming 16px base font
                }
            }
            else if (value.EndsWith("pt"))
            {
                var sizeStr = value.Substring(0, value.Length - 2);
                if (float.TryParse(sizeStr, out float size))
                {
                    style = style.LineHeight(size / 12); // Assuming 12pt base font
                }
            }
            
            return style;
        }

        private static TextStyle ApplyLetterSpacing(TextStyle style, string value)
        {
            value = value.ToLower().Trim();
            
            if (value.EndsWith("px"))
            {
                var sizeStr = value.Substring(0, value.Length - 2);
                if (float.TryParse(sizeStr, out float size))
                {
                    style = style.LetterSpacing(size);
                }
            }
            else if (value.EndsWith("pt"))
            {
                var sizeStr = value.Substring(0, value.Length - 2);
                if (float.TryParse(sizeStr, out float size))
                {
                    style = style.LetterSpacing(size);
                }
            }
            // Handle unitless numbers
            else if (float.TryParse(value, out float size))
            {
                style = style.LetterSpacing(size);
            }
            
            return style;
        }

        private static TextStyle ApplyWordSpacing(TextStyle style, string value)
        {
            value = value.ToLower().Trim();
            
            if (value.EndsWith("px"))
            {
                var sizeStr = value.Substring(0, value.Length - 2);
                if (float.TryParse(sizeStr, out float size))
                {
                    style = style.WordSpacing(size);
                }
            }
            else if (value.EndsWith("pt"))
            {
                var sizeStr = value.Substring(0, value.Length - 2);
                if (float.TryParse(sizeStr, out float size))
                {
                    style = style.WordSpacing(size);
                }
            }
            // Handle unitless numbers
            else if (float.TryParse(value, out float size))
            {
                style = style.WordSpacing(size);
            }
            
            return style;
        }
    }
}
