using System;
using System.Threading.Tasks;

namespace SudokuSolver
{
    internal class SudokuSolver
    {
        private int[,] initialBoard;
        private int[,] sequentialBoard;
        private int[,] parallelBoard;
        private int[,] solution;
        private bool solutionFound;
        private int size;

        public SudokuSolver(int[,] board)
        {
            initialBoard = board;
            size = board.GetLength(0);
        }

        public int[,] solveSequential()
        {
            sequentialBoard = initialBoard.Clone() as int[,];
            if (solveSequentialInternal(0, 0))
            {
                return sequentialBoard;
            } else
            {
                return null;
            }
        }

        public int[,] solveParallel()
        {
            parallelBoard = initialBoard.Clone() as int[,];
            solution = null;
            solutionFound = false;

            //Task t = Task.Run(() => solveParallelInternal(parallelBoard, 0, 0));
            //t.Wait();
            solveParallelInternal(parallelBoard, 0, 0);

            return solution;
        }

        private bool solveSequentialInternal(int row, int col)
        {
            // if reached end of rows => finished
            if (row == size)
            {
                return true;
            }

            // if reached end of column => take next row
            if (col == size)
            {
                return solveSequentialInternal(row + 1, 0);
            }

            // if cell is not empty => skip it
            if (sequentialBoard[row, col] != 0)
            {
                return solveSequentialInternal(row, col + 1);
            }

            // for each possible value (1-9) try to place it => rerun recursive with next cell
            for (int val = 1; val <= size; val++)
            {
                if (canPlaceValue(sequentialBoard, row, col, val))
                {
                    sequentialBoard[row, col] = val;

                    if (solveSequentialInternal(row, col + 1))
                    {
                        return true;
                    }

                    sequentialBoard[row, col] = 0;
                }
            }

            return false;
        }

        private void solveParallelInternal(int[,] board, int row, int col)
        {
            if (solutionFound)
            {
                return;
            }
            // if reached end of rows => finished
            if (row == size)
            {
                solutionFound = true;
                solution = board;
                return;
            }

            // if reached end of column => take next row
            if (col == size)
            {
                solveParallelInternal(board, row + 1, 0);
                return;
            }

            // if cell is not empty => skip it
            if (board[row, col] != 0)
            {
                solveParallelInternal(board, row, col + 1);
                return;
            }

            //// for each possible value (1-9) try to place it => rerun recursive with next cell
            //for (int val = 1; val <= size; val++)
            //{
            //    if (canPlaceValue(parallelBoard, row, col, val))
            //    {
            //        parallelBoard[row, col] = val;

            //        // spawn new task for new cell
            //        Task<bool> t = Task.Run(() => solveParallelInternal(row, col + 1));

            //        if (t.Result)
            //            return true;

            //        parallelBoard[row, col] = 0;
            //    }
            //}

            // for each possible value (1-9) try to place it => rerun recursive with next cell
            Parallel.For(1, size + 1, (val) =>
            {
                if (!solutionFound && canPlaceValue(board, row, col, val))
                {
                    int[,] boardCopy = board.Clone() as int[,];
                    boardCopy[row, col] = val;

                    // spawn new task for new cell
                    //Task t = Task.Run(() => solveParallelInternal(boardCopy, row, col + 1));
                    //t.Wait();
                    solveParallelInternal(boardCopy, row, col + 1);
                }
            });
        }

        private bool canPlaceValue(int[,] board, int row, int col, int val)
        {
            // check column and row of the placement
            for (int x = 0; x < size; x++)
            {
                if (board[row, x] == val || board[x, col] == val)
                    return false;
            }

            // check sub box of the placement
            int regionSize = (int)Math.Sqrt(size);
            int startRow = row - row % regionSize; // first row of the subbox
            int startCol = col - col % regionSize; // first col of the subbox

            for (int i = 0; i < regionSize; i++)
            {
                for (int j = 0; j < regionSize; j++)
                {
                    if (board[i + startRow, j + startCol] == val)
                        return false;
                }
            }

            return true;
        }
    }
}
