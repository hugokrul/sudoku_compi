using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sudoku_compi
{
    public struct Swap 
    {
        public int Score = -1000;
        public Coord Coord1;
        public Coord Coord2;
        public int newValCoord1;
        public int newValCoord2;
        public int newHValRow1;
        public int newHValRow2;
        public int newHValCol1;
        public int newHValCol2;

        public Swap(Coord coord1, Coord coord2)
        {
            Coord1 = coord1;
            Coord2 = coord2;
        }
    }

}