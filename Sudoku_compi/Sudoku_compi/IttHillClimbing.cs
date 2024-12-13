using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Diagnostics;

namespace Sudoku_compi 
{
    /// <summary>
    ///  Class <c>IttHillClimbing</c> is the class in which the algorithm is implemented and ran from.
    /// </summary>
    public class IttHillClimbing
    {
        // Run stats
        public Stopwatch SW = new Stopwatch();
        public TimeSpan Elapsed;
        public int AttemptsNeeded;
        public int TotalSwaps = 0;

        // Algo settings
        private int MaxAttempts = 100;
        private int OptimumCeiling = 100;
        private int RandomSwaps = 20;

        // Algo board
        public Board Board;
        public Random rnd = new Random();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sBoard">The Sudoku board to solve.</param>
        public IttHillClimbing(Board sBoard) 
        {
            Board = sBoard;
        }

        /// <summary>
        /// Start the algorithm and solve the Sudoku.
        /// </summary>
        public void Run()
        {
            SW = new Stopwatch();
            SW.Start();

            // Number of attempts to solve the Sudoku puzzle so far
            // One attempt is swapping till an optimum once; so reaching a local optimum and performing a random walk
            int attempts = 1;

            while (attempts <= MaxAttempts) {
                //First swap till optimum is reached
                SwapTilOptimum(OptimumCeiling);

                // Don't do random walk in last iteration (as it will just scramble the board)
                if (attempts == MaxAttempts)
                {
                    Console.WriteLine("Was not able to solve the board.");
                    break;
                }
                // Board has been solved
                else if (Board.BoardHScore == 0)
                {
                    AttemptsNeeded = attempts;
                    Console.WriteLine("Succesfully solved!");
                    break;
                }
                else
                // Do random walk if board has not yet been solved
                {
                    RandomWalk(RandomSwaps);
                }
                attempts++;
            }
            SW.Stop();
            Elapsed = SW.Elapsed;
        }

        /// <summary>
        /// Implementation of Hill Climbing. In a loop randomly selects 3x3 block on the board and calculates the best swap in this block.
        /// Only commits swaps resulting in a equally good or improved heuristic value of the board. Loop is limited by 'ceiling' parameter. 
        /// </summary>
        /// <param name="ceiling">Maximum length of sequence of selected swaps that do not improve the heuristic value of the board.</param>
        private void SwapTilOptimum(int ceiling) //ceiling determines how many long the swapping will continue without a score delta above 0
        {
            int count = 0;

            while (count < ceiling)
            {
                if (Board.BoardHScore == 0) { break; }
                //Select random 3x3 box
                int vIndex = rnd.Next(0, 3);
                int hIndex = rnd.Next(0, 3);

                List<Swap> swaps = new List<Swap>();

                foreach ((Coord, Coord) swap in Board.AllSwaps[vIndex,hIndex])
                {
                    //Transform to Swap objects
                    swaps.Add(new Swap(swap.Item1,swap.Item2,Board));
                }

                int delta = -1;
                //This list will hold the swap(s) with the highest delta
                List<Swap> doSwaps = new();

                foreach (Swap s in swaps)
                {
                    if (s.Score > delta)
                    {
                        //If a better swap is found the old swaps need to be removed
                        doSwaps.Clear();
                        doSwaps.Add(s);
                        delta = s.Score;
                    }

                    if (s.Score == delta)
                    {
                        doSwaps.Add(s);
                    }
                }

                //Select random swap and commit
                if (delta >= 0)
                {
                    int rIndex = rnd.Next(doSwaps.Count);
                    Swap s = doSwaps[rIndex];
                    Board.CommitSwap(s);
                    TotalSwaps++;
                }

                //If there is an improvement, start count over
                if (delta > 0) { count = 0; }
                //If score stays the same, count 1 up
                else { count++; }
            }
        }

        /// <summary>
        /// Performs a random walk on the board by randomly selecting 3x3 block and performing a random swap without constraint on heuristic value.
        /// </summary>
        /// <param name="amount">Amount of random swaps to make.</param>
        private void RandomWalk(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                List<(Coord,Coord)> legalSwaps = Board.AllSwaps[rnd.Next(3),rnd.Next(3)];
                int randomIndex = rnd.Next(legalSwaps.Count);
                Swap randomSwap = new Swap(legalSwaps[randomIndex].Item1, legalSwaps[randomIndex].Item2, Board);
                Board.CommitSwap(randomSwap);
                TotalSwaps++;
            }
        }
    }
}