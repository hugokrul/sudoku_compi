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
        public IttHillClimbing(Board sBoard) 
        {
            board = sBoard;
            ILS();
        }

        private void ILS()
        {
            List<(Coord,Coord)> swaps = new List<(Coord,Coord)>();
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
    }
}