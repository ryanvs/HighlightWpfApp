# HighlightWpfApp

Examples of Highlighting a Search Term in Text using WPF

There were several examples on StackedOverflow, so I wanted to build an example application to see the differences between the approaches. I added basic support for case-insensitive highlighting, but not for 'whole word only'.

I prefer to use bevahiors derived from `Microsoft.Xaml.Behaviors.Behavior`, so I created the behavior from the Attached Property version.

## Known Bugs

1. The `FlowDocumentHighlighter` is not highlighting the correct offset after the initial match.
2. `HighlightingTextBlock` is not working probably due to Theming/ResourceDictionary issues

## TODO

1. Add configurable styles to `HighlightTerm` and `HighlightTermBehavior` to avoid hard-coding the matching and non-matching text.
2. Add highlight by whole word only.
3. Add UI to test `HighlightComparison` functionality for case-sensitive and case-insensitive searching.
4. Add support for plain and `RegEx` modes.
5. Build a version that works with TextBox or RichTextBox.
6. Improve multilingual examples and support.
