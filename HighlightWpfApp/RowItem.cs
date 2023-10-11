namespace HighlightWpfApp
{
    public class RowItem : ObservableObject
    {
        private int? _Index;

        public int? Index
        {
            get => _Index;
            set => Set(ref _Index, value);
        }

        private string _Text = string.Empty;

        public string Text
        {
            get => _Text;
            set => Set(ref _Text, value ?? string.Empty);
        }
    }
}
