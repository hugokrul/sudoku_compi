namespace Sudoku_compi
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Board board = new Board();

            board.LoadBoard(@"../../../testBoards1.txt");
            board.PrintBoard();
        }
    }
}
