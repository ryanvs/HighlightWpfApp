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

namespace HighlightWpfApp
{
    public class HighlightTextBehavior : Behavior<TextBlock>
    {
        private volatile bool _isTextChanging;
        private volatile bool _pendingTextChanged;


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

            // Fire an event to highlight when control is attached/data bound
            OnTextChanged(this, new DependencyPropertyChangedEventArgs());

            // Attach to get Text changes
            if (this.AssociatedObject != null)
            {
                // When Text changes it throws System.ExecutionEngineException: Exception of type 'System.ExecutionEngineException' was thrown.
                // Intentionally disabled for now...
                var descriptor = DependencyPropertyDescriptor.FromProperty(TextBlock.TextProperty, typeof(TextBlock));
                descriptor.AddValueChanged(this.AssociatedObject, TextBlockChanged);
            }
        }

        protected override void OnDetaching()
        {
            if (this.AssociatedObject != null)
            {
                var descriptor = DependencyPropertyDescriptor.FromProperty(TextBlock.TextProperty, typeof(TextBlock));
                descriptor.RemoveValueChanged(this.AssociatedObject, TextBlockChanged);
            }

            base.OnDetaching();
        }

        private void TextBlockChanged(object sender, EventArgs e)
        {
            if (_isTextChanging)
            {
                Debug.WriteLine("TextBlockChanged: EXITING _isTextChanging = true");
                return;
            }

            var dispatcher = this.AssociatedObject?.Dispatcher;
            if (dispatcher != null)
            {
                _pendingTextChanged = true;
                Debug.WriteLine("TextBlockChanged: DELAYING _pendingTextChanged = true");

                dispatcher.BeginInvoke(
                    (Action)(() =>
                    { 
                        if (_pendingTextChanged)
                        {
                            Debug.WriteLine("TextBlockChanged: EXEC _pendingTextChanged = true");
                            OnTextChanged();
                        }
                        else
                        {
                            Debug.WriteLine("TextBlockChanged: SKIPPED _pendingTextChanged = false");
                        }
                    }),
                    System.Windows.Threading.DispatcherPriority.Normal,
                    null);
            }
        }

        private void OnTextChanged()
        {
            var textBlock = this.AssociatedObject;
            if (textBlock != null)
            {
                string text = textBlock.Text;
                string highlightText = this.HighlightText;
                var comparison = this.HighlightComparision;

                // BUG: This causes changes to SourceText to be missed. There is still
                // a bug in normal usage, but it will update when HighlightText changes
                if (_isTextChanging)
                {
                    Debug.WriteLine("OnTextChanged: SKIPPED _isTextChanging = true");
                    return;
                }

                _isTextChanging = true;
                Debug.WriteLine("OnTextChanged: EXEC _isTextChanging = true");
                SetTextBlockTextAndHighlightTerm(textBlock, text, highlightText, comparison);
                _isTextChanging = false;
                _pendingTextChanged = false;
                Debug.WriteLine("OnTextChanged: FINISHED _isTextChanging = false");
            }
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HighlightTextBehavior behavior)
            {
                behavior.OnTextChanged();
            }
        }

        public static string GetText(FrameworkElement frameworkElement)
        {
            return (string)frameworkElement.GetValue(TextBlock.TextProperty);
        }

        public static StringComparison GetHighlightComparision(FrameworkElement frameworkElement)
        {
            return (StringComparison)frameworkElement.GetValue(HighlightComparisionProperty);
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
            else if (TextIsNotContainingTermToBeHighlighted(text, highlightText, comparison))
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

        private static bool TextIsNotContainingTermToBeHighlighted(string text, string termToBeHighlighted, StringComparison comparison)
        {
            return text.IndexOf(termToBeHighlighted, comparison) < 0;
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

        private static void AddHighlightedPartToTextBlock(TextBlock textBlock, string part)
        {
            textBlock.Inlines.Add(new Run { Text = part, FontWeight = FontWeights.ExtraBold });
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
