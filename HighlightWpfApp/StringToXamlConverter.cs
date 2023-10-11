using System;
using System.Globalization;
using System.IO;
using System.Security;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xml;

namespace HighlightWpfApp
{
    /// <summary>
    /// Converts a string containing valid XAML into WPF objects.
    /// </summary>
    /// <see href="https://web.archive.org/web/20180308102102/http://underground.infovark.com/2011/03/03/highlighting-query-terms-in-a-wpf-textblock/"/>
    [ValueConversion(typeof(string), typeof(object))]
    public sealed class StringToXamlConverter : IValueConverter
    {
        /// <summary>
        /// Returns a delimited string with sections matching highlightText
        /// delimited by: beforeText|~S~|highlightText|~E~|afterText
        /// </summary>
        /// <param name="sourceText">Incoming source text</param>
        /// <param name="highlightText">Search term to highlight</param>
        /// <param name="comparison">Comparison mode for case sensitivity</param>
        /// <returns></returns>
        public static string ConvertToDelimitedText(string sourceText, string highlightText, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (string.IsNullOrEmpty(sourceText) || string.IsNullOrEmpty(highlightText))
            {
                return sourceText;
            }

            var builder = new StringBuilder(string.Empty);
            int previous = 0;
            int found = sourceText.IndexOf(highlightText, previous, comparison);
            while (found >= 0)
            {
                builder.Append(sourceText.Substring(previous, found - previous));
                builder.Append("|~S~|");
                builder.Append(sourceText.Substring(found, highlightText.Length));
                builder.Append("|~E~|");
                previous = found + highlightText.Length;
                found = sourceText.IndexOf(highlightText, previous, comparison);
            }

            if (previous < sourceText.Length)
            {
                builder.Append(sourceText.Substring(previous, sourceText.Length - previous));
            }

            return builder.ToString();
        }

        /// <summary>
        /// Converts a string containing valid XAML into WPF objects. The text
        /// needs to be preprocessed with |~S~|highlightText|~E~| to be
        /// displayed properly.
        /// 
        /// NOTE: Must have a Style with x:Key=highlight
        /// to properly style the matched text.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        /// <returns>A WPF object.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string input = value as string;
            if (input != null)
            {
                string escapedXml = SecurityElement.Escape(input);
                string withTags = escapedXml.Replace("|~S~|", "<Run Style=\"{DynamicResource highlight}\">");
                withTags = withTags.Replace("|~E~|", "</Run>");

                var builder = new StringBuilder();
                builder.Append("<TextBlock xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xml:space=\"preserve\" TextWrapping=\"Wrap\">");
                builder.Append(withTags);
                builder.Append("</TextBlock>");
                string wrappedInput = builder.ToString();

                using (var stringReader = new StringReader(wrappedInput))
                using (var xmlReader = XmlReader.Create(stringReader))
                {
                    return XamlReader.Load(xmlReader);
                }
            }

            return null;
        }

        /// <summary>
        /// Converts WPF framework objects into a XAML string.
        /// </summary>
        /// <param name="value">The WPF Famework object to convert.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        /// <returns>A string containg XAML.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("This converter cannot be used in two-way binding.");
        }
    }
}
