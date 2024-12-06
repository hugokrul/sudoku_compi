using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sudoku_compi
{
    public struct Swap 
    {
        // The total score of the swap, meaning the sum of delta scores of each line; positive is good
        public int Score = -1000;
        // The coordinate of the first number to swap
        public Coord Coord1;
        // The coordinate of the second number to swap
        public Coord Coord2;
        // The value that is to be placed at the coordinate of the first number if the swap is committed; number that is at coordinates of coord2
        // This field is just for ease of use and overview because otherwise a temp var has to be used in CommitSwap function.
        public int newValCoord1;
        // The value that is to be placed at the coordinate of the second number if the swap is committed 
        public int newValCoord2;
        // The new heuristic value of the first affected row if the swap is committed
        public int newHValRow1;
        // The new heuristic value of the second affected row if the swap is committed
        public int newHValRow2;
        // The new heuristic value of the first affected column if the swap is committed
        public int newHValCol1;
        // The new heuristic value of the second affected column if the swap is committed
        public int newHValCol2;

        public Swap(Coord coord1, Coord coord2)
        {
            Coord1 = coord1;
            Coord2 = coord2;
        }

        public override string ToString()
        {
            return $"Coord1: {Coord1.ToString()}, Coord2: {Coord2.ToString()}";
        }
    }

}