using System;

namespace Core
{
    public class CustomMatrix
    {
        public static (int, int) TranslatePosition(int row, int column, OrdinalDirection ordinalDirection)
        {
            return ordinalDirection switch
            {
                OrdinalDirection.North => (row - 1, column),
                OrdinalDirection.NorthEast => (row - 1, column + 1),
                OrdinalDirection.East => (row, column + 1),
                OrdinalDirection.SouthEast => (row + 1, column + 1),
                OrdinalDirection.South => (row + 1, column),
                OrdinalDirection.SouthWest => (row + 1, column - 1),
                OrdinalDirection.West => (row, column - 1),
                OrdinalDirection.NorthWest => (row - 1, column - 1),
                _ => throw new ArgumentException("Invalid OrdinalDirection enum value"),
            };
        }

        public readonly int Rows;
        public readonly int Columns;

        public CustomMatrix(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            Collapsed = new char[rows * columns];
        }

        private char[] Collapsed { get; }

        public void SetValueAt(int row, int column, char value)
        {
            Collapsed[row * Columns + column] = value;
        }

        public char GetValueAt(int row, int column)
        {
            return Collapsed[row * Columns + column];
        }

        public bool PositionIsOccupied(int row, int column)
        {
            return GetValueAt(row, column) != default;
        }

        public bool PositionIsOutOfRange(int row, int column)
        {
            return 0 > row || row >= Rows || 0 > column || column >= Columns;
        }

        public void FillEmptyWithRandom(string characterSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ")
        {
            Random random = new Random();

            for (int i = 0; i < Collapsed.Length; i++)
            {
                if (Collapsed[i] == default)
                {
                    int randIndex = random.Next(characterSet.Length);
                    Collapsed[i] = characterSet[randIndex];
                }
            }
        }

        public void FillEmptyWithPlaceholder()
        {
            for (int i = 0; i < Collapsed.Length; i++)
            {
                if (Collapsed[i] == default)
                {
                    Collapsed[i] = ' ';
                }
            }
        }
    }
}
