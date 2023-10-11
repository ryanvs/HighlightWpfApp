using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace HighlightWpfApp
{
    public class HighlightTextBehavior : Behavior<TextBlock>
    {
        public string SourceText
        {
            get { return (string)GetValue(SourceTextProperty); }
            set { SetValue(SourceTextProperty, value); }
        }

        public static readonly DependencyProperty SourceTextProperty =
            DependencyProperty.Register(
                nameof(SourceText),
                typeof(string),
                typeof(HighlightTextBehavior),
                new FrameworkPropertyMetadata(string.Empty, OnTextChanged));


        public string HighlightText
        {
            get { return (string)GetValue(HighlightTextProperty); }
            set { SetValue(HighlightTextProperty, value ?? string.Empty); }
        }

        public static readonly DependencyProperty HighlightTextProperty =
            DependencyProperty.Register(
                nameof(HighlightText),
                typeof(string),
                typeof(HighlightTextBehavior),
                new FrameworkPropertyMetadata(string.Empty, OnTextChanged));


        public StringComparison HighlightComparision
        {
            get { return (StringComparison)GetValue(HighlightComparisionProperty); }
            set { SetValue(HighlightComparisionProperty, value); }
        }

        public static readonly DependencyProperty HighlightComparisionProperty =
            DependencyProperty.Register(
                nameof(HighlightComparision),
                typeof(StringComparison),
                typeof(HighlightTextBehavior),
                new FrameworkPropertyMetadata(StringComparison.OrdinalIgnoreCase, OnTextChanged));

        protected override void OnAttached()
        {
            base.OnAttached();
            this.OnTextChanged();
        }

        private void OnTextChanged()
        {
            var textBlock = this.AssociatedObject;
            if (textBlock != null)
            {
                string text = this.SourceText;
                string highlightText = this.HighlightText;
                var comparison = this.HighlightComparision;

                SetTextBlockTextAndHighlightTerm(textBlock, text, highlightText, comparison);
            }
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HighlightTextBehavior behavior)
            {
                behavior.OnTextChanged();
            }
        }

        public static StringComparison GetHighlightComparision(FrameworkElement frameworkElement)
        {
            return (StringComparison)frameworkElement.GetValue(HighlightComparisionProperty);
        }

        private static void SetTextBlockTextAndHighlightTerm(TextBlock textBlock, string text, string highlightText, StringComparison comparison)
        {
            textBlock.SetValue(TextBlock.TextProperty, text);
            textBlock.Inlines.Clear();

            if (TextIsEmpty(text))
                return;

            if (string.IsNullOrEmpty(highlightText))
            {
                AddTextToTextBlock(textBlock, text);
            }
            else if (TextDoesNotContainTermToHighlight(text, highlightText, comparison))
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
            return string.IsNullOrEmpty(text);
        }

        private static bool TextDoesNotContainTermToHighlight(string text, string termToBeHighlighted, StringComparison comparison)
        {
            return text == null || text.IndexOf(termToBeHighlighted, comparison) < 0;
        }

        private static void AddPartToTextBlockAndHighlightIfNecessary(TextBlock textBlock, string termToBeHighlighted, string textPart, StringComparison comparison)
        {
            if (string.Equals(textPart, termToBeHighlighted, comparison))
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
