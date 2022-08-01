using System;

namespace Core
{
    public class CustomMatrix
    {
        public static (int x, int y) TranslatePosition(int row, int column, OrdinalDirection ordinalDirection)
        {
            switch (ordinalDirection)
            {
                case OrdinalDirection.North:
                    return (row - 1, column);
                case OrdinalDirection.NorthEast: 
                    return (row - 1, column + 1);
                case OrdinalDirection.East: 
                    return (row, column + 1);
                case OrdinalDirection.SouthEast: 
                    return (row + 1, column + 1);
                case OrdinalDirection.South: 
                    return (row + 1, column);
                case OrdinalDirection.SouthWest: 
                    return (row + 1, column - 1);
                case OrdinalDirection.West: 
                    return (row, column - 1);
                case OrdinalDirection.NorthWest:  
                    return (row - 1, column - 1);
                default:
                    throw new ArgumentException("Invalid OrdinalDirection enum value");
            };
        }

        public int Rows { get; }
        public int Columns { get; }

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
