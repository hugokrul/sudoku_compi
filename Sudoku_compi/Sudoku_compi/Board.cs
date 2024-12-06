using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace Sudoku_compi
{
    public class Board
    {
        static Random rnd = new Random();
        public int[,] board = new int[9, 9];
        public string BoardName;
        public string BoardFile;
        public bool[,] boolMatrix = new bool[9, 9]; //True if it contains a fixed value
        // Keeps track of the scores of the rows i.e. rowHVals[1] gives the score of row 1 
        public int[] rowHVals = new int[9];
        // Keeps track of the scores of the columns
        public int[] colHVals = new int[9];
        // Tracks total score of the board; -1 is placeholder
        public int boardHScore = -1;

        public Board(string boardFile)
        {
            BoardFile = boardFile;
            // loads the board
            // file content has to match: @"(Grid  \d\d*\r\n)+( \d){81,81}"
            LoadBoard();
        }

        // At the start of the program, calculate heuristic values of all rows and columns and the begin score of the board
        public void InitHValArrays() 
        {            
            // Initiate rowHVals and colHVals; 
            for (int i = 0; i < 9; i++)
            {
                int[] row = Enumerable.Range(0, 9)
                    .Select(j => board[j, i])
                    .ToArray();

                rowHVals[i] = lineHeuristic(row);

                int[] col = Enumerable.Range(0, 9)
                    .Select(j => board[i, j])
                    .ToArray();

                colHVals[i] = lineHeuristic(col);
            }
        }

        // Calculates the total heuristic value of the board; call at the start after InitHValArrays
        public void HValBoard ()
        {
            int tempBoardScore = 0;
            for (int i = 0; i < 9; i++)
            {
                tempBoardScore += rowHVals[i] + colHVals[i];
            }
            boardHScore = tempBoardScore;
        }

        public int lineHeuristic(int[] line)
        {
            var lineInumerable = from x in line
                                 where x > 0
                                 select x;
            return 9 - lineInumerable.Distinct().Count();
        }

        public int boardHeuristic(int[,] b)
        {
            int h = 0;
            List<int> line = [];

            for (int i = 0; i < b.GetLength(0); i++)
            {
                for (int j = 0; j < b.GetLength(1); j++)
                {
                    line.Add(b[i, j]);
                }
                h += lineHeuristic(line.ToArray());
                line.Clear();
            }

            for (int i = 0; i < b.GetLength(0); i++)
            {
                for (int j = 0; j < b.GetLength(1); j++)
                {
                    line.Add(b[j,i]);
                }
                h += lineHeuristic(line.ToArray());
                line.Clear();
            }

            return h;
        }

        // returns a board where all the zeros are filled in with numbers available in that box
        public int[,] fillBoard(int[,] b)
        {
            for (int boxX = 0; boxX < 3; boxX++)
            {
                for (int boxY = 0; boxY < 3; boxY++)
                {
                    List<int> availableNumbers = [1, 2, 3, 4, 5, 6, 7, 8, 9];

                    // removes a number from the list of available numbers, if that number is already in the box
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            var element = b[i + 3*boxX, j + 3*boxY];
                            if (availableNumbers.Contains(element)) availableNumbers.Remove(element);
                        }
                    }

                    // filles the 0 with random numbers available
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            var element = b[i+ 3*boxX, j + 3*boxY];
                            if (element == 0)
                            {
                                var randomNumber = availableNumbers[rnd.Next(availableNumbers.Count)];
                                b[i+ 3*boxX, j+3*boxY] = randomNumber;
                                availableNumbers.Remove(randomNumber);
                            }
                        }
                    }
                }
            }
            return board;
        }


        public void RandomWalk(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                (int, int) box = (rnd.Next(3), rnd.Next(3));
                List<(Coord, Coord)> legalSwaps = getLegalSwaps(box);
                int randomIndex = rnd.Next(legalSwaps.Count);
                Swap randomSwap = CoordsToSwap(legalSwaps[randomIndex].Item1, legalSwaps[randomIndex].Item2);
                CommitSwap(randomSwap);
            }
        }

        public List<(Coord, Coord)> getLegalSwaps((int, int) box) //Here box is a coordinate which points to the specific 3x3 square we want the swaps from. Structured as x(vertical), y(horizontal) where 0,0 is topleft
        {
            List<(int, int)> unfixedCoordinates = [];
            List <(Coord, Coord)> swaps = [];
            int X = box.Item1 * 3;
            int Y = box.Item2 * 3;

            for (int x = X; x < X + 3;  x++)
            {
                for (int y = Y; y < Y + 3; y++)
                {
                    if (!boolMatrix[x, y]) unfixedCoordinates.Add((x, y));
                }
            }

            int c = unfixedCoordinates.Count;
            for (int i = 0; i < c; i++)
            {
                (int, int) c1 = unfixedCoordinates[i];
                for (int j = i + 1; j < c; j++)
                {
                    (int, int) c2 = unfixedCoordinates[j];
                    swaps.Add((new Coord(c1.Item1, c1.Item2), new Coord(c2.Item1, c2.Item2)));
                }
            }

            return swaps;
        }

        public void LoadBoard()
        {
            // checks if the file exists
            if (File.Exists(BoardFile))
            {
                // checks if the content of the file matches:
                // Grid  digitdigit
                //  81 digits with spaces in front
                string boardString = File.ReadAllText(BoardFile);
                if (Regex.IsMatch(File.ReadAllText(BoardFile), @"(Grid  \d\d*\r\n)+( \d){81,81}$"))
                {
                    string[] strings = File.ReadAllLines(BoardFile);
                    // The boardname is the first element in the string
                    BoardName = strings[0];
                    // the next elements are the numbers in the board, the first element is "" because of a space at the front, we skip that space
                    string[] BoardNumbers = strings[1].Split(' ').Skip(1).ToArray();

                    for (int i = 0; i < board.GetLength(0); i++)
                    {
                        for (int j = 0; j < board.GetLength(1); j++)
                        {
                            // first x digits are the first line, next x digits are the next line, etc
                            int val = int.Parse(BoardNumbers[board.GetLength(0) * i + j]);
                            board[i, j] = val;

                            if (val == 0) { boolMatrix[i, j] = false; }
                            else { boolMatrix[i, j] = true; }
                        }
                    }
                } else
                {
                    Console.WriteLine("non correct format");
                }
            } else
            {
                Console.WriteLine("file does not exist");
            }
        }

        // checks if a line is correct, i.e., it contains no duplicate digits except 0
        public bool checkLine(List<int> line)
        {
            // C# has no map, so
            // with inumerable we can easy map over the digits
            var lineInumerable = from x in line
                                 where x > 0
                                 select x;

            List<int> lineList = lineInumerable.ToList();
            // checks if there are no duplicates without 0
            return lineList.Count == lineList.Distinct().Count();
        }

        // returns if the state board is possible
        // input is a board, so we can check it for future states too
        public bool checkBoard(int[,] b)
        {
            List<bool> checkedHorizontalLines = [];
            List<bool> checkedVerticalLines = [];

            List<int> horizontalLine = [];
            List<int> verticalLine = [];
            
            // checks all the horizontal lines with checkLine
            for (int i = 0; i < b.GetLength(0); i++)
            {
                for (int j = 0; j < b.GetLength(1); j++)
                {
                    horizontalLine.Add(b[i, j]);
                }
                checkedHorizontalLines.Add(checkLine(horizontalLine));
                horizontalLine.Clear();
            }

            // checks all the vertical lines with checkLine
            for (int i = 0; i < b.GetLength(0); i++)
            {
                for (int j = 0; j < b.GetLength(1); j++)
                {
                    verticalLine.Add(b[j, i]);
                }
                checkedVerticalLines.Add(checkLine(verticalLine));
                verticalLine.Clear();
            }

            // if all lines are correct, i.e., the list is full of Trues and bigger then 0
            return  checkedHorizontalLines.TrueForAll(x => x) && checkedHorizontalLines.Count > 0 
                    && checkedVerticalLines.TrueForAll(x => x) && checkedVerticalLines.Count > 0;
        }

        // prints the board in a pretty way
        public void PrintBoard()
        {
            Console.WriteLine(@"/ - - - - - - - - - - - \");
            for (int i = 0;i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (j % 3 == 0 && j != 0) Console.Write(" |");
                    if (j == 0) Console.Write($"| {board[i, j]}");
                    else if (j == 8) Console.Write($" {board[i, j]} |");
                    else Console.Write($" {board[i, j]}");
                }
                Console.WriteLine();
                if ((i + 1) % 3 == 0 && i != 8) Console.WriteLine("  - - - - - - - - - - -  ");
            }
            Console.WriteLine(@"\ - - - - - - - - - - - /");
        }

        // Creates a Swap struct voor a given swap of two coords
        public Swap CoordsToSwap(Coord coord1, Coord coord2)
        {
            Swap swap = new Swap(coord1, coord2);
            int totalDelta = 0;

            swap.newValCoord1 = board[coord2.X,coord2.Y];
            swap.newValCoord2 = board[coord1.X,coord1.Y];

            // If the two coords share the same column
            if (coord1.X == coord2.X)
            {
                // Value of column does not change so just set the new column values to the current value 
                swap.newHValCol1 = colHVals[coord1.X];
                swap.newHValCol2 = colHVals[coord1.X];
            }
            else
            {
                // In this column the value of coord1 is replaced by the value of coord2
                int[] col1 = Enumerable.Range(0, 9)
                    .Select(i => board[coord1.X, i])
                    .ToArray();
                
                // Replace value of coord1 with value of coord2
                col1[coord1.Y] = board[coord2.X, coord2.X];
                // Calculate new value of col1
                int hValCol1 = lineHeuristic(col1);
                // Calculate difference between old and new column value;
                // formula = old - new; a positive value means the row has gotten closer to completion
                int col1Delta = colHVals[coord1.X] - hValCol1;
                totalDelta += col1Delta;

                swap.newHValCol1 = hValCol1;

                // In this row the value of coord2 is replaced with the value of coord1
                int[] col2 = Enumerable.Range(0, 9)
                    .Select(i => board[i, coord2.X])
                    .ToArray();
                
                col2[coord2.Y] = board[coord1.X, coord1.Y];
                int hValCol2 = lineHeuristic(col2);
                int col2Delta = colHVals[coord2.X] - hValCol2;
                totalDelta += col2Delta;

                swap.newHValCol2 = hValCol2;
            }

            // If the two coords share the same row
            if (coord1.Y == coord2.Y)
            {
                // Value of the row does not changeso just set the new row values to the current row value
                swap.newHValRow1 = rowHVals[coord1.Y];
                swap.newHValRow2 = rowHVals[coord1.Y];
            }
            else 
            {
                // In this row the value of coord1 is replaced with the value of coord2
                int[] row1 = Enumerable.Range(0, 9)
                    .Select(i => board[i, coord1.Y])
                    .ToArray();
                
                row1[coord1.X] = board[coord2.X, coord2.Y];
                int hValRow1 = lineHeuristic(row1);
                int row1Delta = rowHVals[coord1.Y] - hValRow1;
                totalDelta += row1Delta;

                swap.newHValRow1 = hValRow1;

                // In this row the value of coord2 is replaced with the value of coord1
                int[] row2 = Enumerable.Range(0, 9)
                    .Select(i => board[i, coord2.Y])
                    .ToArray();
                
                row2[coord2.X] = board[coord1.X, coord1.Y];
                int hValRow2 = lineHeuristic(row2);
                int row2Delta = rowHVals[coord2.Y] - hValRow2;
                totalDelta += row2Delta;

                swap.newHValRow2 = hValRow2;
            }
            swap.Score = totalDelta;

            return swap;
        }

        public void CommitSwap(Swap swap)
        {
            // Swap the values on the board
            board[swap.Coord1.X, swap.Coord1.Y] = swap.newValCoord1;
            board[swap.Coord2.X, swap.Coord2.Y] = swap.newValCoord2;

            // Set new values of rows
            rowHVals[swap.Coord1.Y] = swap.newHValRow1;
            rowHVals[swap.Coord2.Y] = swap.newHValRow2;

            // Set new values of columns
            colHVals[swap.Coord1.X] = swap.newHValCol1;
            colHVals[swap.Coord2.X] = swap.newHValCol2;

            // Finally, adjust the total score of the board
            boardHScore -= swap.Score;
        }
    }
}
