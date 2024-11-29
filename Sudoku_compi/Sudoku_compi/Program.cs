namespace Sudoku_compi
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Board board = new Board(@"../../../testBoards3.txt");

            board.PrintBoard();
            Console.WriteLine(board.checkBoard(board.board));
        }
    }
}
