using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sudoku_compi
{
    public struct Coord
    {
        public Coord(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X;
        public int Y;
    }
}