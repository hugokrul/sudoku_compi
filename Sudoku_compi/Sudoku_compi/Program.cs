using System.IO;

namespace Sudoku_compi
{
    internal class Program
    {
        static void Main(string[] args)
        {
            for (int x = 0; x < 10; x++)
            {
                Console.WriteLine(x);
                List<double> result = [];
                List<int> resultFailed = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0];

                for (int i = 1; i <= 10; i++)
                {
                    Console.WriteLine(i * 100);
                    List<double> intermediate = [];
                    List<int> failed = [];
                    for (int j = 1; j <= 5; j++)
                    {
                        Board board = new Board(@"../../../testBoards" + j + ".txt");

                        IttHillClimbing algo = new IttHillClimbing(board);
                        algo.RandomSwaps = i * 5;
                        algo.Run();
                        intermediate.Add(algo.Elapsed.TotalSeconds);
                        failed.Add(algo.failed);
                    }
                    result.Add(intermediate.Sum());
                    resultFailed[i - 1] = resultFailed[i - 1] + failed.Sum();
                    intermediate.Clear();
                }
                string createText = "[" + String.Join(", ", result) + "]," + Environment.NewLine + "[" + String.Join(", ", resultFailed) + "]," + Environment.NewLine;
                File.AppendAllText(@"..\..\..\results.txt", createText);
            }
        }
    }
}
