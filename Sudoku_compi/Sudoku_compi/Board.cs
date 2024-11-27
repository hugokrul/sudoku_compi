using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku_compi
{
    public class Board
    {
        public int[,] board = new int[9, 9];
        public string BoardName;
        public string BoardFile;

        public Board(string boardFile)
        {
            BoardFile = boardFile;
        }

        public void LoadBoard()
        {
            if (File.Exists(BoardFile))
            {
                string[] strings = File.ReadAllLines(BoardFile);
                BoardName = strings[0];
                string[] BoardNumbers = strings[1].Split(' ').Skip(1).ToArray();

                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        board[i, j] = int.Parse(BoardNumbers[9*i+j]);
                    }
                }
            } else
            {
                Console.WriteLine("file does not exist");
            }
        }

        // checks if there are more then 2 numbers bigger then 0 in line
        public bool checkLine(List<int> line)
        {
            var lineInumerable = from x in line
                                 where x > 0
                                 select x;

            List<int> lineList = lineInumerable.ToList();
            return lineList.Count == lineList.Distinct().Count();
        }

        public bool checkBoard()
        {
            List<bool> checkedHorizontalLines = [];
            List<bool> checkedVerticalLines = [];

            List<int> horizontalLine = [];
            List<int> verticalLine = [];
            
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    horizontalLine.Add(board[i, j]);
                }
                checkedHorizontalLines.Add(checkLine(horizontalLine));
                horizontalLine.Clear();
            }

            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    verticalLine.Add(board[j, i]);
                }
                checkedVerticalLines.Add(checkLine(verticalLine));
                verticalLine.Clear();
            }

            return checkedHorizontalLines.TrueForAll(x => x) && checkedVerticalLines.TrueForAll(x => x);
        }

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
    }
}
