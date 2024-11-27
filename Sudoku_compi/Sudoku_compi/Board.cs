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

        public Board()
        {

        }

        public void LoadBoard(string fileName)
        {
            if (File.Exists(fileName))
            {
                string[] strings = File.ReadAllLines(fileName);
                BoardName = strings[0];
                string[] BoardNumbers = strings[1].Split(' ').Skip(1).ToArray();

                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        board[i, j] = int.Parse(BoardNumbers[i*j]);
                    }
                }
            } else
            {
                Console.WriteLine("file does not exist");
            }
        }

        public void PrintBoard()
        {
            for (int i = 0;i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (j % 3 == 0 && j != 0) Console.Write("| ");
                    Console.Write($"|{board[i, j]}");
                }
                Console.WriteLine("|");
                if ((i+1) % 3 == 0) Console.WriteLine();
            }
        }
    }
}
