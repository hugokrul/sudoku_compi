﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Sudoku_compi
{
    public class Board
    {
        public int[,] board = new int[9, 9];
        public string BoardName;
        public string BoardFile;
        public bool[,] boolMatrix = new bool[9, 9]; //True if it contains a fixed value
        public int[] rowHVals = new int[9];
        public int[] colHVals = new int[9];

        public Board(string boardFile)
        {
            BoardFile = boardFile;
            // loads the board
            // file content has to match: @"(Grid  \d\d*\r\n)+( \d){81,81}"
            LoadBoard();
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
            List<bool> checkedBoxes = [];

            List<int> horizontalLine = [];
            List<int> verticalLine = [];
            List<int> boxes = [];
            
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

            //Check the 9 boxes
            for (int bh = 0; bh < 3; bh++)
            {
                for (int bv = 0; bv < 9; bv++)
                {
                    for (int i = 3 * bh; i < 3; i++)
                    {
                        for (int j = 3 * bv; j < 3; j++)
                        {
                            boxes.Add(board[j, i]);
                        }
                    }
                    checkedBoxes.Add(checkLine(boxes));
                    boxes.Clear();
                }
            }

            // if all lines are correct, i.e., the list is full of Trues and bigger then 0
            return checkedHorizontalLines.TrueForAll(x => x) && checkedHorizontalLines.Count > 0 && checkedVerticalLines.TrueForAll(x => x) && checkedVerticalLines.Count > 0 && checkedBoxes.TrueForAll(x => x) && checkedBoxes.Count > 0;
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

        public List<int[]> coordsToLines(Tuple<int,int> coord1, Tuple<int,int> coord2)
        {
            List<int[]> lines = new List<int[]>();

            // In this row the value of coord1 is replaced by the value of coord2
            int[] row1 = Enumerable.Range(0, 9)
                .Select(i => board[i, coord1.Item2])
                .ToArray();
            
            row1[coord1.Item1] = board[coord2.Item1, coord2.Item2];
            lines.Add(row1);

            // If the y coordinates are different, two rows will be affected
            if (coord1.Item2 != coord2.Item2)
            {
                // In this row the value of coord2 is replaced with the value of coord1
                int[] row2 = Enumerable.Range(0, 9)
                    .Select(i => board[i, coord2.Item2])
                    .ToArray();
                
                row2[coord2.Item1] = board[coord1.Item1, coord1.Item2];
                lines.Add(row2);
            }

            // In this column the value of coord1 is replaced by the value of coord2
            int[] col1 = Enumerable.Range(0, 9)
                .Select(i => board[coord1.Item1, i])
                .ToArray();
            
            col1[coord1.Item2] = board[coord2.Item1, coord2.Item2];
            lines.Add(col1);

            // If the x coordinates are different, two columns will be affected
            if (coord1.Item1 != coord2.Item1)
            {
                // In this row the value of coord2 is replaced with the value of coord1
                int[] col2 = Enumerable.Range(0, 9)
                    .Select(i => board[i, coord2.Item1])
                    .ToArray();
                
                col2[coord2.Item2] = board[coord1.Item1, coord1.Item2];
                lines.Add(col2);
            }
            
            // lines contains the new columns and rows after the swap in the order:
            // First the rows, starting with row of coord1 then coord2
            // then the columns, starting with column of coord1 then coord2
            return lines;
        }
    }
}
