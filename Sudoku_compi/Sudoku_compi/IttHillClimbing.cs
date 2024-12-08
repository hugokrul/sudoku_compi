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
            List<(Coord,Coord)> swaps = board.getLegalSwaps((1,1)); // For now 1,1; needs be randomized
            // Placeholder swap
            Swap placeHolder = new Swap(new Coord(-1,-1), new Coord(-1,-1));
            Swap bestSwap = placeHolder;

            foreach ((Coord,Coord) swap in swaps)
            {
                Swap newSwap = board.CoordsToSwap(swap.Item1, swap.Item2);
                if (newSwap.Score >= bestSwap.Score)
                    bestSwap = newSwap;
            }

            // Als de beste swap het bord verslechtert dan ...
            if (bestSwap.Score < 0)
            {
                // Denk aan random walk iterator verhogen etc 
            }
            else 
            {
                board.CommitSwap(bestSwap);
            }
        }

        public void swapTilOptimum(int ceiling, Board board) //ceiling determines how many long the swapping will continue without a score delta above 0
        {
            int count = 0;

            while (count < ceiling)
            {
                int vIndex = rnd.Next(0, 3);
                int hIndex = rnd.Next(0, 3);

                List<(Coord, Coord)> cc = board.getLegalSwaps((vIndex, hIndex));
                List<Swap> swaps = new List<Swap>();

                foreach ((Coord, Coord) swap in cc)
                {
                    swaps.Add(board.CoordsToSwap(swap.Item1, swap.Item2));
                }
                
            }


        }
    }
}