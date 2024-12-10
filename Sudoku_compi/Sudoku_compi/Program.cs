using System.Security.Cryptography.X509Certificates;

namespace Sudoku_compi
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Board board = new Board(@"../../../testBoards5.txt");  

            IttHillClimbing algo = new IttHillClimbing(board);
            algo.Run();
            algo.Board.PrintBoard();
            Console.WriteLine("Attempts taken to solve: " + algo.AttemptsNeeded);
            Console.WriteLine("Time taken to solve: " + algo.Elapsed);
            Console.WriteLine("Total swaps made: " + algo.TotalSwaps);
        }
    }
}
