using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;

namespace Core
{
    public class Wordsearch
    {
        public Wordsearch()
        {
            Title = string.Empty;
            Words = new List<Word>();
        }

        #region Properties

        public string Title { get; set; }
        public List<Word> Words { get; set; }

        #endregion

        #region Serialization

        public XDocument Serialize()
        {
            return new XDocument(
                new XElement("Wordsearch",
                    new XElement("Title", Title),
                    new XElement("Words", Words.Select(x => 
                        new XElement("Word", 
                            new XElement("Text", x.Text),
                            new XElement("StartColumn", x.StartColumn),
                            new XElement("StartRow", x.StartRow),
                            new XElement("Direction", x.Direction)
             )))));
        }

        public static Wordsearch Deserialize(XDocument doc)
        {
            var result = new Wordsearch();

            var root = doc.Element("Wordsearch");
            if (root == null)
            {
                return result;
            }

            result.Title = root.Element("Title")?.Value ?? "";

            var wordsElement = root.Element("Words");
            if (wordsElement != null)
            {
                foreach (var wordElem in wordsElement.Elements("Word"))
                {
                    result.Words.Add(new Word
                    {
                        Text = wordElem.Element("Text")?.Value ?? "",
                        StartColumn = int.TryParse(wordElem.Element("StartColumn")?.Value, out var sc) ? sc : 0,
                        StartRow = int.TryParse(wordElem.Element("StartRow")?.Value, out var sr) ? sr : 0,
                        Direction = Enum.TryParse<OrdinalDirection>(wordElem.Element("Direction")?.Value, out var di) ? di : OrdinalDirection.North
                    });
                }
            }
            return result;
            
        }

        #endregion

        #region ConvertingToImage

        #region Constants
        private static readonly Font LetterFont = new Font("Arial", 18);
        private static readonly Font TitleFont = new Font("Arial", 35);
        private static readonly Font WordFont = new Font("Arial", 15);
        private static readonly PointF TitleLocation = new PointF(298f, 60f);
        private static readonly StringFormat CentreFormat = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        private static readonly PointF GridZeroZero = new PointF(49.4f, 130f);
        private const float GridSpacing = 26f;
        private static readonly PointF WordZeroZero = new PointF(60f, 660f);
        private const float WordRowSpacing = 20f;
        private const float WordColumnSpacing = 180f;
        #endregion

        public Bitmap ToBitmap(WordsearchRenderOption options)
        {
            // Build Matrix
            var matrix = BuildMatrixExceptBlanks();
            if ((options & WordsearchRenderOption.FillEmptyCellsWithRandomLetters) == WordsearchRenderOption.FillEmptyCellsWithRandomLetters)
            {
                matrix.FillEmptyWithRandom();
            }
            else
            {
                matrix.FillEmptyWithPlaceholder();
            }

            Bitmap bitmap;
            // Load Base Image
            if ((options & WordsearchRenderOption.VisibleGrid) == WordsearchRenderOption.VisibleGrid)
            {
                bitmap = Resources.PreviewBackgroundImage;
            }
            else
            {
                bitmap = Resources.ExportBackgroundImage;
            }


            using (Graphics graphics = Graphics.FromImage(bitmap)) 
            {
                // Draw Title
                graphics.DrawString(Title, TitleFont, Brushes.Black, TitleLocation, CentreFormat);

                // Draw Letters in Cell Positions
                for (int i = 0; i < matrix.Rows; i++)
                {
                    for (int j = 0; j < matrix.Columns; j++)
                    {
                        var point = new PointF(GridZeroZero.X + GridSpacing * j, GridZeroZero.Y + GridSpacing * i);
                        graphics.DrawString(matrix.GetValueAt(i, j).ToString(), LetterFont, Brushes.Black, point, CentreFormat);
                    }
                }

                // Draw Words
                int count = 0;
                int columns = 3;
                foreach (Word word in Words)
                {
                    int col = count % columns;
                    int row = count / columns;
                    var wordPoint = new PointF(WordZeroZero.X + WordColumnSpacing * col, WordZeroZero.Y + WordRowSpacing * row);
                    graphics.DrawString
                    (
                        word.Text.ToUpper().Trim(),
                        WordFont,
                        Brushes.Black,
                        wordPoint
                    );
                    count++;
                }
            }

            return bitmap;
        }

        private CustomMatrix BuildMatrixExceptBlanks()
        {
            CustomMatrix matrix = new CustomMatrix(20, 20);

            foreach (Word word in Words)
            {
                int column = word.StartColumn;
                int row = word.StartRow;
                foreach (char character in word.Text.RemoveWhitespace().ToUpper())
                {
                    if (matrix.PositionIsOutOfRange(row, column))
                    {
                        break;
                    }
                    else if (matrix.PositionIsOccupied(row, column))
                    {
                        // just leave as is.
                    }
                    else
                    {
                        matrix.SetValueAt(row, column, character);
                    }
                    (row, column) = CustomMatrix.TranslatePosition(row, column, word.Direction);
                }
            }
            return matrix;
        }

        #endregion


        #region Randomisation

        private readonly struct RowAndCol
        {
            public RowAndCol(int row, int col)
            {
                Row = row;
                Col = col;
            }
            public readonly int Row;
            public readonly int Col;
        }

        /// <summary>
        /// Try to generate a random wordsearch from the given words. If this runs into a situation where there is no valid cells
        /// to put a word, then it will restart. It will do this for the number of times given in the <see cref="attemptLimit"/> before it gives up.
        /// </summary>
        /// <param name="words">Words to put in the wordsearch</param>
        /// <param name="wordsearch">The produced wordsearch</param>
        /// <param name="attemptLimit">The number of attempts allowed before giving up</param>
        /// <returns>Whether the wordsearch was completed successfully within the number of attempts allowed</returns>
        public static bool TryGenerateRandom(IEnumerable<string> words, out Wordsearch wordsearch, int attemptLimit = 1000)
        {
            wordsearch = new Wordsearch
            {
                // Process the strings into words
                Words = words.Select(i => i.Trim())
                             .Where(i => i.Length != 0)
                             .Select(i => new Word() { Text = i })
                             .ToList()
            };

            // Confirm that maximum word length is not exceeded
            if (wordsearch.Words.Where(i => i.Text.RemoveWhitespace().Length > 20).Any())
            {
                return false;
            }


            Random random = new Random();

            

            // Limit the attempt number. With many words (expecially long ones) it may struggle to fit them

            int attempt = 0;
            bool complete = false;
            while (attempt < attemptLimit && !complete) // max attempts will take around 1 second
            {
                attempt++;

                // Generate an empty matrix
                CustomMatrix matrix = new CustomMatrix(20, 20);

                // if the end of this loop is reached without encountering the break
                // then the wordsearch generation is complete
                complete = true;
                foreach (Word word in wordsearch.Words)
                {
                    var validCells = new List<RowAndCol>();

                    // Get a random ordinal direction
                    OrdinalDirection direction = (OrdinalDirection)random.Next(0, 8);

                    string wordWithoutWhitespace = word.Text.RemoveWhitespace();
                    int wlen = wordWithoutWhitespace.Length;

                    // Initialise the cells for consideration as the whole matrix
                    int rowstart = 0;
                    int rowend = matrix.Rows;
                    int colstart = 0;
                    int colend = matrix.Columns;

                    // Remove cells around the edge from consideration based on the length and direction of the word
                    switch (direction)
                    {
                        case OrdinalDirection.North:
                            rowstart = rowstart + wlen - 1;
                            break;
                        case OrdinalDirection.NorthEast:
                            rowstart = rowstart + wlen - 1;
                            colend = colend - wlen + 1;
                            break;
                        case OrdinalDirection.East:
                            colend = colend - wlen + 1;
                            break;
                        case OrdinalDirection.SouthEast:
                            rowend = rowend - wlen + 1;
                            colend = colend - wlen + 1;
                            break;
                        case OrdinalDirection.South:
                            rowend = rowend - wlen + 1;
                            break;
                        case OrdinalDirection.SouthWest:
                            rowend = rowend - wlen + 1;
                            colstart = colstart + wlen - 1;
                            break;
                        case OrdinalDirection.West:
                            colstart = colstart + wlen - 1;
                            break;
                        case OrdinalDirection.NorthWest:
                            rowstart = rowstart + wlen - 1;
                            colstart = colstart + wlen - 1;
                            break;
                    }

                    // using the adjusted strart and end row and column, check if that each cell is valid
                    for (int i = rowstart; i < rowend; i++)
                    {
                        for (int j = colstart; j < colend; j++)
                        {
                            int row = i;
                            int col = j;
                            bool occupied = false;
                            // using the words direction, for each letter in the word, check if the cell it would be in is availiable
                            // the cell is also valid if they are the same letter
                            for (int c = 0; c < wlen; c++)
                            {
                                if (matrix.PositionIsOccupied(row, col) && wordWithoutWhitespace[c] != matrix.GetValueAt(row, col))
                                {
                                    occupied = true;
                                    break;
                                };
                                (row, col) = CustomMatrix.TranslatePosition(row, col, direction);
                            }
                            if (!occupied)
                            {
                                validCells.Add(new RowAndCol(i, j));
                            }
                        }
                    }

                    // if there is no valid position found for the word
                    // break from the loop with an incomplete marker
                    // which will begin a new attempt
                    if (validCells.Count < 1)
                    {
                        complete = false;
                        break;
                    }

                    #region idea prob never use
                    // pick a random segment to avoid bias towards parallel close together words
                    /*
                    int randomSegmentRowStart = random.Next(0, 15);
                    int randomSegmentColStart = random.Next(0, 15);

                    List<int[]> segmentValidCells = validCells.Where
                    (i => 
                        i[0] >= randomSegmentRowStart &&
                        i[0] < randomSegmentRowStart + 5 &&
                        i[0] >= randomSegmentColStart &&
                        i[0] < randomSegmentColStart + 5
                    ).ToList();

                    if (segmentValidCells.Count > 0)
                    {
                        validCells = segmentValidCells;
                    }
                    */
                    #endregion
                    
                    // Choose a random cell from all of the valid options
                    RowAndCol cell = validCells[random.Next(0, validCells.Count)];

                    word.Direction = direction;
                    word.StartRow = cell.Row;
                    word.StartColumn = cell.Col;

                    // Input the word into the matrix at the chosen position 
                    int rw = word.StartRow;
                    int cl = word.StartColumn;
                    foreach (char chr in word.Text.RemoveWhitespace())
                    {
                        matrix.SetValueAt(rw, cl, chr);
                        (rw, cl) = CustomMatrix.TranslatePosition(rw, cl, word.Direction);
                    }
                }
            }

            // if complete == false here that means the attempt limit was reached without success
            if (!complete) 
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}
