using System;

namespace AutoWordsearch.Core
{
    [Flags]
    public enum WordsearchRenderOption
    {
        VisibleGrid = 1,
        FillEmptyCellsWithRandomLetters = 2,
    }
}
