using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Numerics;

namespace Sudoku_compi 
{
    public class IttHillClimbing
    {
        Board board;
        static Random rnd = new Random();
        public IttHillClimbing(Board sBoard) 
        {
            board = sBoard;
            ILS();
        }

        private void ILS()
        {
            //First swap til optimum is reached
            swapTilOptimum(1000, board);
            //Then RandomWalk
            if (board.boardHScore != 0)
            {
                //Randomwalkfunction
            }
        }

        public void swapTilOptimum(int ceiling, Board board) //ceiling determines how many long the swapping will continue without a score delta above 0
        {
            int count = 0;

            while (count < ceiling)
            {   
                if (board.boardHScore == 0) { break; }
                //Select random 3x3 box
                int vIndex = rnd.Next(0, 3);
                int hIndex = rnd.Next(0, 3);

                List<(Coord, Coord)> cc = board.getLegalSwaps((vIndex, hIndex));
                List<Swap> swaps = new List<Swap>();

                foreach ((Coord, Coord) swap in cc)
                {
                    //Transform to Swap objects
                    swaps.Add(board.CoordsToSwap(swap.Item1, swap.Item2));
                }

                int delta = -1;
                //This list will hold the swap(s) with the highest delta
                List<Swap> doSwaps = new();

                foreach (Swap s in  swaps)
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
                    board.CommitSwap(s);
                }

                //If there is an improvement, start count over
                if (delta > 0) { count = 0; }
                //If score stays the same, count 1 up
                else if (delta == 0) { count++; }
                //If the swaps only have negative delta, stop the while
                else { count = ceiling; }
            }


        }
    }
}