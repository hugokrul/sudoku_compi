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

        public Swap(Coord coord1, Coord coord2, Board b)
        {
            Coord1 = coord1;
            Coord2 = coord2;

            int totalDelta = 0;

            newValCoord1 = b.board[coord2.X,coord2.Y];
            newValCoord2 = b.board[coord1.X,coord1.Y];

            // If the two coords share the same column
            if (coord1.X == coord2.X)
            {
                // Value of column does not change so just set the new column values to the current value 
                newHValCol1 = b.ColHVals[coord1.X];
                newHValCol2 = b.ColHVals[coord1.X];
            }
            else
            {
                // In this column the value of coord1 is replaced by the value of coord2
                int[] col1 = Enumerable.Range(0, 9)
                    .Select(i => b.board[coord1.X, i])
                    .ToArray();
                
                // Replace value of coord1 with value of coord2
                col1[coord1.Y] = b.board[coord2.X, coord2.Y];
                // Calculate new value of col1
                int hValCol1 = b.lineHeuristic(col1);
                // Calculate difference between old and new column value;
                // formula = old - new; a positive value means the row has gotten closer to completion
                int col1Delta = b.ColHVals[coord1.X] - hValCol1;
                totalDelta += col1Delta;

                newHValCol1 = hValCol1;

                // In this row the value of coord2 is replaced with the value of coord1
                int[] col2 = Enumerable.Range(0, 9)
                    .Select(i => b.board[coord2.X, i])
                    .ToArray();
                
                col2[coord2.Y] = b.board[coord1.X, coord1.Y];
                int hValCol2 = b.lineHeuristic(col2);
                int col2Delta = b.ColHVals[coord2.X] - hValCol2;
                totalDelta += col2Delta;

                newHValCol2 = hValCol2;
            }

            // If the two coords share the same row
            if (coord1.Y == coord2.Y)
            {
                // Value of the row does not changeso just set the new row values to the current row value
                newHValRow1 = b.RowHVals[coord1.Y];
                newHValRow2 = b.RowHVals[coord1.Y];
            }
            else 
            {
                // In this row the value of coord1 is replaced with the value of coord2
                int[] row1 = Enumerable.Range(0, 9)
                    .Select(i => b.board[i, coord1.Y])
                    .ToArray();
                
                row1[coord1.X] = b.board[coord2.X, coord2.Y];
                int hValRow1 = b.lineHeuristic(row1);
                int row1Delta = b.RowHVals[coord1.Y] - hValRow1;
                totalDelta += row1Delta;

                newHValRow1 = hValRow1;

                // In this row the value of coord2 is replaced with the value of coord1
                int[] row2 = Enumerable.Range(0, 9)
                    .Select(i => b.board[i, coord2.Y])
                    .ToArray();
                
                row2[coord2.X] = b.board[coord1.X, coord1.Y];
                int hValRow2 = b.lineHeuristic(row2);
                int row2Delta = b.RowHVals[coord2.Y] - hValRow2;
                totalDelta += row2Delta;

                newHValRow2 = hValRow2;
            }
            Score = totalDelta;
        }
    }

}