using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Documents;

namespace HighlightWpfApp
{
    public class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            _HighlightText = "the";

            _Examples = new ObservableCollection<ExampleText>(ExampleTexts.Data);
            _SelectedExample = _Examples[0];
            _SourceText = _SelectedExample.Text;

            CreateDelimited();
            CreateItems();
            CreateFlowDocument();
        }

        private ObservableCollection<ExampleText> _Examples;

        public ObservableCollection<ExampleText> Examples
        {
            get => _Examples;
            private set => Set(ref _Examples, value);
        }

        private ExampleText _SelectedExample;

        public ExampleText SelectedExample
        {
            get => _SelectedExample;
            set
            {
                if (Set(ref _SelectedExample, value))
                {
                    SourceText = value?.Text ?? string.Empty;
                }
            }
        }

        private string _SourceText = string.Empty;

        public string SourceText
        {
            get => _SourceText;
            set
            {
                if (Set(ref _SourceText, value ?? string.Empty))
                {
                    CreateDelimited();
                    CreateItems();
                    CreateFlowDocument();
                }
            }
        }

        private string _HighlightText = string.Empty;

        public string HighlightText
        {
            get => _HighlightText;
            set
            {
                if (Set(ref _HighlightText, value ?? string.Empty))
                {
                    CreateDelimited();
                    CreateFlowDocument();
                }
            }
        }

        #region Delimited for StringToXamlConverter
        private string _DelimitedText = string.Empty;

        public string DelimitedText
        {
            get => _DelimitedText;
            set => Set(ref _DelimitedText, value ?? string.Empty);
        }

        private void CreateDelimited()
        {
            DelimitedText = StringToXamlConverter.ConvertToDelimitedText(SourceText, HighlightText);
        }
        #endregion

        #region Collection for DataGrid
        private ObservableCollection<RowItem> _Items;

        public ObservableCollection<RowItem> Items
        {
            get => _Items;
            private set => Set(ref _Items, value);
        }

        private void CreateItems()
        {
            int index = 0;
            var parts = _SourceText
                .Split(' ')
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(s => new RowItem() { Text = s.Replace("\r\n", " "), Index = index++ });

            Items = new ObservableCollection<RowItem>(parts);
        }
        #endregion

        #region FlowDocument
        private FlowDocument _Document;

        public FlowDocument Document
        {
            get => _Document;
            private set => Set(ref _Document, value);
        }

        private void CreateFlowDocument()
        {
            var flowDocument = new FlowDocument();
            flowDocument.ColumnWidth = 999999; // https://stackoverflow.com/a/5721686
            var p = new Paragraph(new Run(SourceText));
            flowDocument.Blocks.Add(p);
            flowDocument.HighlightDocument(HighlightText);
            Document = flowDocument;
        }
        #endregion
    }
}
