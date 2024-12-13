using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Sudoku_compi
{
    public class Board
    {
        static Random rnd = new Random();
        public int[,] board = new int[9, 9];
        public string BoardName;
        public string BoardFile;
        public bool[,] boolMatrix = new bool[9, 9]; //True if it contains a fixed value
        public List<(Coord,Coord)>[,] AllSwaps = new List<(Coord,Coord)>[3,3];
        // Keeps track of the scores of the rows i.e. rowHVals[1] gives the score of row 1 
        public int[] RowHVals = new int[9];
        // Keeps track of the scores of the columns
        public int[] ColHVals = new int[9];
        // Tracks total score of the board; -1 is placeholder
        public int BoardHScore = -1;

        public Board(string boardFile)
        {
            BoardFile = boardFile;
            // loads the board
            // file content has to match: @"(Grid  \d\d*\r\n)+( \d){81,81}"
            List<Coord>[,] mutableCoords = LoadBoard();
            CalcAllSwaps(mutableCoords);
            fillBoard();
            InitHValArrays();
            HValBoard();
        }

        // At the start of the program, calculate heuristic values of all rows and columns
        public void InitHValArrays() 
        {            
            // Initiate rowHVals and colHVals; 
            for (int i = 0; i < 9; i++)
            {
                int[] row = Enumerable.Range(0, 9)
                    .Select(j => board[j, i])
                    .ToArray();

                RowHVals[i] = lineHeuristic(row);

                int[] col = Enumerable.Range(0, 9)
                    .Select(j => board[i, j])
                    .ToArray();

                ColHVals[i] = lineHeuristic(col);
            }
        }

        // Calculates the total heuristic value of the board; call at the start after InitHValArrays
        public void HValBoard ()
        {
            int tempBoardScore = 0;
            for (int i = 0; i < 9; i++)
            {
                tempBoardScore += RowHVals[i] + ColHVals[i];
            }
            BoardHScore = tempBoardScore;
        }

        public int lineHeuristic(int[] line)
        {
            var lineInumerable = from x in line
                                 where x > 0
                                 select x;
            return 9 - lineInumerable.Distinct().Count();
        }

        // returns a board where all the zeros are filled in with numbers available in that box
        public void fillBoard()
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
                            var element = board[i + 3*boxX, j + 3*boxY];
                            if (availableNumbers.Contains(element)) availableNumbers.Remove(element);
                        }
                    }

                    // filles the 0 with random numbers available
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            var element = board[i+ 3*boxX, j + 3*boxY];
                            if (element == 0)
                            {
                                var randomNumber = availableNumbers[rnd.Next(availableNumbers.Count)];
                                board[i+ 3*boxX, j+3*boxY] = randomNumber;
                                availableNumbers.Remove(randomNumber);
                            }
                        }
                    }
                }
            }
        }

        public void CalcAllSwaps(List<Coord>[,] mutableCoords) 
        {
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (AllSwaps[x,y] == null)
                        AllSwaps[x,y] = new List<(Coord, Coord)>();

                    int c = mutableCoords[x, y].Count;
                    for (int i = 0; i < c; i++)
                    {
                        Coord c1 = mutableCoords[x,y][i];
                        for (int j = i + 1; j < c; j++)
                        {
                            Coord c2 = mutableCoords[x,y][j];
                            AllSwaps[x,y].Add((c1,c2));
                        }
                    }
                }
            }
        }

        public List<Coord>[,] LoadBoard()
        {
            // checks if the file exists
            if (File.Exists(BoardFile))
            {
                // checks if the content of the file matches:
                // Grid  digitdigit
                //  81 digits with spaces in front
                string boardString = File.ReadAllText(BoardFile);
                if (Regex.IsMatch(File.ReadAllText(BoardFile), @"^Grid  \d{2}\r?\n(\s\d){81}$", RegexOptions.Multiline))
                {
                    string[] strings = File.ReadAllLines(BoardFile);
                    // The boardname is the first element in the string
                    BoardName = strings[0];
                    // the next elements are the numbers in the board, the first element is "" because of a space at the front, we skip that space
                    string[] BoardNumbers = strings[1].Split(' ').Skip(1).ToArray();

                    List<Coord>[,] MutableCoords = new List<Coord>[3,3];

                    for (int i = 0; i < board.GetLength(0); i++)
                    {
                        for (int j = 0; j < board.GetLength(1); j++)
                        {
                            // first x digits are the first line, next x digits are the next line, etc
                            int val = int.Parse(BoardNumbers[board.GetLength(0) * i + j]);
                            board[i, j] = val;

                            if (MutableCoords[i / 3, j / 3] == null)
                                MutableCoords[i / 3, j / 3] = new List<Coord>();

                            if (val == 0) 
                            { 
                                boolMatrix[i, j] = false;                                 
                                MutableCoords[i / 3, j / 3].Add(new Coord(i,j));
                            }
                            else 
                            { 
                                boolMatrix[i, j] = true; 
                            }
                        }
                    }

                    return MutableCoords;
                } else
                {
                    Console.WriteLine("non correct format");
                    return null;
                }
            } else
            {
                Console.WriteLine("file does not exist");
                return null;
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

        public void CommitSwap(Swap swap)
        {
            // Swap the values on the board
            board[swap.Coord1.X, swap.Coord1.Y] = swap.newValCoord1;
            board[swap.Coord2.X, swap.Coord2.Y] = swap.newValCoord2;

            // Set new values of rows
            RowHVals[swap.Coord1.Y] = swap.newHValRow1;
            RowHVals[swap.Coord2.Y] = swap.newHValRow2;

            // Set new values of columns
            ColHVals[swap.Coord1.X] = swap.newHValCol1;
            ColHVals[swap.Coord2.X] = swap.newHValCol2;

            // Finally, adjust the total score of the board
            BoardHScore -= swap.Score;
        }
    }
}
