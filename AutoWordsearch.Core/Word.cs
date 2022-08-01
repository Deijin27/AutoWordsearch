
namespace AutoWordsearch.Core
{
    public class Word
    {
        public string Text { get; set; }
        public int StartColumn { get; set; }
        public int StartRow { get; set; }
        public OrdinalDirection Direction { get; set; }
    }

}
