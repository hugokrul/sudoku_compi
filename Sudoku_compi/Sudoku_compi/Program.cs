namespace Sudoku_compi
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Board board = new Board(@"../../../testBoards1.txt");

            board.LoadBoard();
            board.PrintBoard();
            Console.WriteLine(board.checkBoard());
        }
    }
}
