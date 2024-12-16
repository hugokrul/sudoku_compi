using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sudoku_compi
{
    /// <summary>
    /// Struct that represents the coordinate of a cell on the board.
    /// </summary>
    public struct Coord
    {
        public Coord(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X;
        public int Y;

        public override string ToString()
        {
            return $"x: {X} y: {Y}";
        }
    }
}