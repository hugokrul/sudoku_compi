using System.Security.Cryptography.X509Certificates;

namespace Sudoku_compi
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Board board = new Board(@"../../../testBoards5.txt");

            IttHillClimbing algo = new IttHillClimbing(board);

            board.PrintBoard();
            Console.WriteLine(board.checkBoard(board.board));
        }
    }
}
