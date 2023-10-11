using System;
using System.Windows.Documents;
using System.Windows.Media;

namespace HighlightWpfApp
{
    public static class FlowDocumentHighlighter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="document"></param>
        /// <param name="highlightText"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        /// <see cref="https://stackoverflow.com/q/18760702/29762"/>
        public static FlowDocument HighlightDocument(this FlowDocument document, string highlightText, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (string.IsNullOrEmpty(highlightText))
                return document;

            var current = document.ContentStart;
            while (current != null)
            {
                var next = current.GetNextContextPosition(LogicalDirection.Forward);
                if (next == null)
                {
                    break;
                }

                var range = new TextRange(current, next);
                string search = current.GetTextInRun(LogicalDirection.Forward);
                int index = range.Text.IndexOf(highlightText, comparison);
                while (index >= 0)
                {
                    int length = index + highlightText.Length;
                    var start  = current.GetPositionAtOffset(index);
                    var end    = current.GetPositionAtOffset(length);
                    var rng    = new TextRange(start, end);
                    rng.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.Yellow));

                    //start = current.GetPositionAtOffset(length + 1);
                    //if (start.CompareTo(next) >= 0)
                    //    break;

                    //range = new TextRange(start, next);
                    search = current.GetTextInRun(LogicalDirection.Forward);
                    index = range.Text.IndexOf(highlightText, length, comparison);
                }

                current = next;
            }

            return document;
        }
    }
}
