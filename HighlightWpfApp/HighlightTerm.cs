using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace HighlightWpfApp
{
    /// <summary>
    /// A TextBlock Attached Property that highlights a specific 'term'.
    /// </summary>
    public static class HighlightTerm
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached(
                "Text",
                typeof(string),
                typeof(HighlightTerm),
                new FrameworkPropertyMetadata("", OnTextChanged));


        public static readonly DependencyProperty HighlightTextProperty =
            DependencyProperty.RegisterAttached(
                "HighlightText",
                typeof(string),
                typeof(HighlightTerm),
                new FrameworkPropertyMetadata("", OnTextChanged));

        public static readonly StringComparison DefaultComparison = StringComparison.OrdinalIgnoreCase;

        // HighlightComparisionProperty causes a runtime error:
        // System.Windows.Markup.XamlParseException:
        //    The type initializer for 'HighlightWpfApp.HighlightTerm' threw an exception.
        //    ArgumentException: 'HighlightTerm' type must derive from DependencyObject.
        //
        //public static readonly DependencyProperty HighlightComparisionProperty =
        //    DependencyProperty.Register(
        //        "HighlightComparision",
        //        typeof(StringComparison?),
        //        typeof(HighlightTerm),
        //        new FrameworkPropertyMetadata(DefaultComparison, OnTextChanged));


        public static string GetText(FrameworkElement frameworkElement)
            => (string)frameworkElement.GetValue(TextProperty);

        public static void SetText(FrameworkElement frameworkElement, string value)
            => frameworkElement.SetValue(TextProperty, value);

        public static string GetHighlightText(FrameworkElement frameworkElement)
        {
            return (string)frameworkElement.GetValue(HighlightTextProperty);
        }

        public static void SetHighlightText(FrameworkElement frameworkElement, string value)
        {
            frameworkElement.SetValue(HighlightTextProperty, value);
        }

        public static StringComparison? GetHighlightComparision(FrameworkElement frameworkElement)
        {
            return StringComparison.OrdinalIgnoreCase;
            //return (StringComparison)frameworkElement.GetValue(HighlightComparisionProperty);
        }

        public static void GetHighlightComparision(FrameworkElement frameworkElement, StringComparison? value)
        {
            //frameworkElement.SetValue(HighlightComparisionProperty, value);
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBlock textBlock)
                SetTextBlockTextAndHighlightTerm(textBlock, GetText(textBlock), GetHighlightText(textBlock), GetHighlightComparision(textBlock) ?? DefaultComparison);
        }

        private static void SetTextBlockTextAndHighlightTerm(TextBlock textBlock, string text, string highlightText, StringComparison comparison)
        {
            textBlock.Inlines.Clear();

            if (TextIsEmpty(text))
                return;

            if (string.IsNullOrEmpty(highlightText))
            {
                AddTextToTextBlock(textBlock, text);
            }
            else if (TextDoesNotContainHighlightText(text, highlightText, comparison))
            {
                AddPartToTextBlock(textBlock, text);
            }
            else
            {
                var textParts = SplitTextIntoTermAndNotTermParts(text, highlightText, comparison);
                foreach (var textPart in textParts)
                    AddPartToTextBlockAndHighlightIfNecessary(textBlock, highlightText, textPart, comparison);
            }
        }

        private static bool TextIsEmpty(string text)
        {
            return text.Length == 0;
        }

        private static bool TextDoesNotContainHighlightText(string text, string highlightText, StringComparison comparison)
        {
            return text.IndexOf(highlightText, comparison) < 0;
        }

        private static void AddPartToTextBlockAndHighlightIfNecessary(TextBlock textBlock, string highlightText, string textPart, StringComparison comparison)
        {
            if (string.Equals(textPart, highlightText, comparison))
                AddHighlightedPartToTextBlock(textBlock, textPart);
            else
                AddPartToTextBlock(textBlock, textPart);
        }

        private static void AddTextToTextBlock(TextBlock textBlock, string part)
        {
            textBlock.Inlines.Add(new Run { Text = part });
        }

        private static void AddPartToTextBlock(TextBlock textBlock, string part)
        {
            textBlock.Inlines.Add(new Run { Text = part, FontWeight = FontWeights.Light });
        }

        private static readonly SolidColorBrush YellowBrush = new SolidColorBrush(Colors.Yellow);

        private static void AddHighlightedPartToTextBlock(TextBlock textBlock, string part)
        {
            textBlock.Inlines.Add(new Run { Text = part, FontWeight = FontWeights.Bold, Background = YellowBrush });
        }

        public static List<string> SplitTextIntoTermAndNotTermParts(string text, string term, StringComparison comparison)
        {
            if (string.IsNullOrEmpty(text))
                return new List<string>() { string.Empty };

            bool isCaseInsensitive = comparison == StringComparison.OrdinalIgnoreCase
                || comparison == StringComparison.InvariantCultureIgnoreCase
                || comparison == StringComparison.CurrentCultureIgnoreCase;
            bool isInvariant = comparison == StringComparison.InvariantCultureIgnoreCase
                || comparison == StringComparison.InvariantCulture;

            var options = RegexOptions.Multiline;
            if (isCaseInsensitive)
                options |= RegexOptions.IgnoreCase;
            if (isInvariant)
                options |= RegexOptions.CultureInvariant;

            string pattern = $@"({Regex.Escape(term)})";
            return Regex.Split(text, pattern, options)
                        .Where(p => p != string.Empty)
                        .ToList();
        }
    }
}
