using System;
using System.Diagnostics;

namespace SudokuSolver
{
    static class Program
    {
        private const int LENGTH = 9;
        private readonly static int[,] initialBoard = {
            {5, 3, 0, 0, 7, 0, 0, 0, 0},
            {6, 0, 0, 1, 9, 5, 0, 0, 0},
            {0, 9, 8, 0, 0, 0, 0, 6, 0},
            {8, 0, 0, 0, 6, 0, 0, 0, 3},
            {4, 0, 0, 8, 0, 3, 0, 0, 1},
            {7, 0, 0, 0, 2, 0, 0, 0, 6},
            {0, 6, 0, 0, 0, 0, 2, 8, 0},
            {0, 0, 0, 4, 1, 9, 0, 0, 5},
            {0, 0, 0, 0, 8, 0, 0, 7, 9}
        };
        //private readonly static int[,] initialBoard = {
        //    {1, 0, 0, 0, 3, 0, 15, 0, 0, 0, 9, 0, 0, 7, 0, 0},
        //    {3, 0, 6, 9, 0, 10, 0, 0, 1, 7, 4, 5, 8, 0, 12, 15 },
        //    {0, 11, 15, 4, 0, 6, 8, 12, 14, 2, 0, 13, 10, 3, 0, 9 },
        //    {0, 0, 8, 12, 0, 4, 5, 0, 15, 0, 0, 0, 0, 2, 0, 14 },

        //    {0, 8, 3, 0, 13, 0, 9, 0, 0, 0, 12, 11, 1, 14, 2, 0 },
        //    {13, 0, 0, 11, 15, 3, 16, 2, 0, 1, 7, 0, 0, 8, 0, 12 },
        //    {0, 1, 7, 6, 0, 11, 0, 0, 16, 0, 8, 14, 0, 0, 0, 0 },
        //    {10, 0, 12, 0, 14, 1, 0, 8, 0, 0, 13, 15, 0, 4, 7, 0 },

        //    {12, 4, 13, 1, 0, 16, 14, 0, 0, 0, 11, 7, 0, 0, 0, 0 },
        //    {0, 16, 11, 10, 0, 0, 0, 3, 0, 12, 0, 2, 0, 0, 14, 7 },
        //    {0, 0, 5, 3, 9, 2, 0, 7, 13, 0, 0, 16, 0, 0, 11, 4 },
        //    {0, 0, 14, 2, 0, 0, 0, 11, 0, 8, 15, 10, 0, 1, 6, 3 },

        //    {9, 0, 10, 0, 0, 0, 0, 0, 6, 4, 2, 8, 0, 12, 16, 1 },
        //    {0, 0, 0, 7, 0, 5, 2, 0, 0, 0, 0, 0, 15, 0, 4, 0 },
        //    {6, 0, 16, 0, 0, 0, 4, 0, 7, 0, 5, 0, 14, 0, 0, 10 },
        //    {0, 12, 0, 0, 0, 0, 7, 6, 0, 13, 0, 1, 9, 0, 0, 2 }
        //};

        static void Main()
        {
            SudokuSolver sudokuSolver = new SudokuSolver(initialBoard);

            #region Sequential

            Console.WriteLine("Solving Sudoku sequential...");

            Stopwatch swSeq = Stopwatch.StartNew();
            int[,] boardSeq = sudokuSolver.solveSequential();
            swSeq.Stop();

            double timeSeq = Convert.ToDouble(swSeq.ElapsedMilliseconds);

            #endregion

            #region Parallel

            Console.WriteLine("Solving Sudoku parallel...");

            Stopwatch swPar = Stopwatch.StartNew();
            int[,] boardPar = sudokuSolver.solveParallel();
            swPar.Stop();

            double timePar = Convert.ToDouble(swPar.ElapsedMilliseconds);

            #endregion

            #region Output

            Console.WriteLine("Sequential time:     " + timeSeq + "ms");
            Console.WriteLine("Parallel time:       " + timePar + "ms");
            Console.WriteLine("speedup quotient:    " + Math.Round(timeSeq / timePar, 2));
            Console.WriteLine();
            Console.WriteLine("Original board:");
            printBoard(initialBoard);
            Console.WriteLine();
            if (boardSeq != null)
            {
                Console.WriteLine("Sequential board (" + (checkBoard(boardSeq) ? "CORRECT" : "INCORRECT") + "):");
                printBoard(boardSeq);
            } else
            {
                Console.WriteLine("Sequential board: No solution found!");
            }
            Console.WriteLine();
            if (boardPar != null)
            {
                Console.WriteLine("Parallel board: (" + (checkBoard(boardPar) ? "CORRECT" : "INCORRECT") + "):");
                printBoard(boardPar);
            } else
            {
                Console.WriteLine("Parallel board: No solution found!");
            }

            #endregion

            Console.ReadLine();
        }

        private static void printBoard(int[,] board)
        {
            for (int i = 0; i < LENGTH; i++)
            {
                for (int j = 0; j < LENGTH; j++)
                {
                    Console.Write(board[i, j] + " ");
                }
                Console.WriteLine();
            }

        }

        private static bool checkBoard(int[,] board)
        {
            double checkSum = (LENGTH + 1) * ((double)LENGTH / 2); // Gauss sum formula

            for (int i = 0; i < LENGTH; i++)
            {

                // calculate sums of rows and columns
                double sumCol = 0;
                double sumRow = 0;
                for (int j = 0; j < LENGTH; j++)
                {
                    sumCol += int.Parse(board[i, j].ToString());
                    sumRow += int.Parse(board[j, i].ToString());
                }

                // check sums of rows and columns
                if (checkSum != sumCol || checkSum != sumRow)
                {
                    return false;
                }
            }

            // check sub box
            int regionSize = (int)Math.Sqrt(LENGTH);

            for (int i = 0; i < regionSize; i++)
            {
                int topLeftOfSubBoxRow = regionSize * i;

                for (int j = 0; j < regionSize; j++)
                {
                    int topLeftOfSubBoxCol = regionSize * j;
                    double sum = 0;

                    // calculate sum in sub box
                    for (int x = topLeftOfSubBoxRow; x < topLeftOfSubBoxRow + regionSize; x++)
                    {
                        for (int y = topLeftOfSubBoxCol; y < topLeftOfSubBoxCol + regionSize; y++)
                        {
                            sum += int.Parse(board[x, y].ToString());
                        }
                    }

                    if (checkSum != sum)
                    {
                        return false;
                    }
                }

            }

            return true;
        }
    }
}
