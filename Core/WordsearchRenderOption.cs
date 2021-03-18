using System;

namespace Core
{
    [Flags]
    public enum WordsearchRenderOption
    {
        VisibleGrid = 1,
        FillEmptyCellsWithRandomLetters = 2,
    }
}
