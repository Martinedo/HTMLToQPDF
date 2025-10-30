using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace HTMLToQPDF.Utils
{
    internal static class CSSContainerMapper
    {
        public static IContainer ApplyInlineStyles(IContainer container, Dictionary<string, string> cssProperties)
        {
            foreach (var (property, value) in cssProperties)
            {
                container = ApplyProperty(container, property, value);
            }

            return container;
        }

        private static IContainer ApplyProperty(IContainer container, string property, string value)
        {
            return property.ToLower() switch
            {
                "text-align" => ApplyTextAlign(container, value),
                "background" or "background-color" => ApplyBackground(container, value),
                "padding" => ApplyPadding(container, value),
                "padding-left" => ApplyPaddingLeft(container, value),
                "padding-right" => ApplyPaddingRight(container, value),
                "padding-top" => ApplyPaddingTop(container, value),
                "padding-bottom" => ApplyPaddingBottom(container, value),
                "margin" => ApplyMargin(container, value),
                "margin-left" => ApplyMarginLeft(container, value),
                "margin-right" => ApplyMarginRight(container, value),
                "margin-top" => ApplyMarginTop(container, value),
                "margin-bottom" => ApplyMarginBottom(container, value),
                "width" => ApplyWidth(container, value),
                "max-width" => ApplyMaxWidth(container, value),
                "min-width" => ApplyMinWidth(container, value),
                "height" => ApplyHeight(container, value),
                "max-height" => ApplyMaxHeight(container, value),
                "min-height" => ApplyMinHeight(container, value),
                "border" or "border-width" => ApplyBorder(container, value),
                "border-color" => ApplyBorderColor(container, value),
                _ => container // Ignore unsupported properties
            };
        }

        private static IContainer ApplyTextAlign(IContainer container, string value)
        {
            value = value.ToLower().Trim();

            return value switch
            {
                "left" => container.AlignLeft(),
                "center" => container.AlignCenter(),
                "right" => container.AlignRight(),
                "justify" => container, // QuestPDF doesn't have explicit justify for containers
                _ => container
            };
        }

        private static IContainer ApplyBackground(IContainer container, string value)
        {
            var color = CSSParser.ParseColor(value);
            if (color != null)
            {
                container = container.Background(color);
            }
            return container;
        }

        private static IContainer ApplyPadding(IContainer container, string value)
        {
            var size = ParseSize(value);
            if (size.HasValue)
            {
                container = container.Padding(size.Value);
            }
            return container;
        }

        private static IContainer ApplyPaddingLeft(IContainer container, string value)
        {
            var size = ParseSize(value);
            if (size.HasValue)
            {
                container = container.PaddingLeft(size.Value);
            }
            return container;
        }

        private static IContainer ApplyPaddingRight(IContainer container, string value)
        {
            var size = ParseSize(value);
            if (size.HasValue)
            {
                container = container.PaddingRight(size.Value);
            }
            return container;
        }

        private static IContainer ApplyPaddingTop(IContainer container, string value)
        {
            var size = ParseSize(value);
            if (size.HasValue)
            {
                container = container.PaddingTop(size.Value);
            }
            return container;
        }

        private static IContainer ApplyPaddingBottom(IContainer container, string value)
        {
            var size = ParseSize(value);
            if (size.HasValue)
            {
                container = container.PaddingBottom(size.Value);
            }
            return container;
        }

        private static IContainer ApplyMargin(IContainer container, string value)
        {
            var size = ParseSize(value);
            if (size.HasValue)
            {
                container = container.Padding(size.Value); // QuestPDF uses Padding for spacing
            }
            return container;
        }

        private static IContainer ApplyMarginLeft(IContainer container, string value)
        {
            var size = ParseSize(value);
            if (size.HasValue)
            {
                container = container.PaddingLeft(size.Value);
            }
            return container;
        }

        private static IContainer ApplyMarginRight(IContainer container, string value)
        {
            var size = ParseSize(value);
            if (size.HasValue)
            {
                container = container.PaddingRight(size.Value);
            }
            return container;
        }

        private static IContainer ApplyMarginTop(IContainer container, string value)
        {
            var size = ParseSize(value);
            if (size.HasValue)
            {
                container = container.PaddingTop(size.Value);
            }
            return container;
        }

        private static IContainer ApplyMarginBottom(IContainer container, string value)
        {
            var size = ParseSize(value);
            if (size.HasValue)
            {
                container = container.PaddingBottom(size.Value);
            }
            return container;
        }

        private static IContainer ApplyWidth(IContainer container, string value)
        {
            var size = ParseSize(value);
            if (size.HasValue)
            {
                container = container.Width(size.Value);
            }
            return container;
        }

        private static IContainer ApplyMaxWidth(IContainer container, string value)
        {
            var size = ParseSize(value);
            if (size.HasValue)
            {
                container = container.MaxWidth(size.Value);
            }
            return container;
        }

        private static IContainer ApplyMinWidth(IContainer container, string value)
        {
            var size = ParseSize(value);
            if (size.HasValue)
            {
                container = container.MinWidth(size.Value);
            }
            return container;
        }

        private static IContainer ApplyHeight(IContainer container, string value)
        {
            var size = ParseSize(value);
            if (size.HasValue)
            {
                container = container.Height(size.Value);
            }
            return container;
        }

        private static IContainer ApplyMaxHeight(IContainer container, string value)
        {
            var size = ParseSize(value);
            if (size.HasValue)
            {
                container = container.MaxHeight(size.Value);
            }
            return container;
        }

        private static IContainer ApplyMinHeight(IContainer container, string value)
        {
            var size = ParseSize(value);
            if (size.HasValue)
            {
                container = container.MinHeight(size.Value);
            }
            return container;
        }

        private static IContainer ApplyBorder(IContainer container, string value)
        {
            var size = ParseSize(value);
            if (size.HasValue)
            {
                container = container.Border(size.Value);
            }
            return container;
        }

        private static IContainer ApplyBorderColor(IContainer container, string value)
        {
            var color = CSSParser.ParseColor(value);
            if (color != null)
            {
                container = container.BorderColor(color);
            }
            return container;
        }

        /// <summary>
        /// Parses a CSS size value (px, pt, em, etc.) and returns the value in points
        /// </summary>
        private static float? ParseSize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            value = value.ToLower().Trim();

            // Handle px units
            if (value.EndsWith("px"))
            {
                var sizeStr = value.Substring(0, value.Length - 2);
                if (float.TryParse(sizeStr, out float size))
                {
                    return size;
                }
            }
            // Handle pt units
            else if (value.EndsWith("pt"))
            {
                var sizeStr = value.Substring(0, value.Length - 2);
                if (float.TryParse(sizeStr, out float size))
                {
                    return size;
                }
            }
            // Handle unitless numbers (treated as px)
            else if (float.TryParse(value, out float size))
            {
                return size;
            }

            return null;
        }
    }
}
