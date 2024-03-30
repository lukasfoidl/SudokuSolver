using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Sudoku
{
    static class Program
    {
        static StreamReader streamReader;
        const string PATH = @"..\..\Sudoku\";
        const string FILE = "case17.txt";
        const char EMPTY_ENTRY = '*';
        const int LENGTH = 9;

        [STAThread]
        static void Main()
        {
            Console.WriteLine("Reading file " + FILE + "...");

            char[,] board = new char[9, 9];
            bool success = readTestCase(PATH + FILE, board);
            if (!success) return;

            Console.WriteLine("Solving Sudoku sequential...");

            #region Sequential

            char[,] boardSeq = (char[,]) board.Clone();
            Stopwatch swSeq = Stopwatch.StartNew();

            Sequential sequential = new Sequential(LENGTH, EMPTY_ENTRY);
            sequential.solveSudoku(boardSeq);

            swSeq.Stop();

            double timeSeq = Convert.ToDouble(swSeq.ElapsedMilliseconds);

            #endregion

            Console.WriteLine("Solving Sudoku parallel...");

            #region Parallel

            char[,] boardPar = (char[,])board.Clone();
            Stopwatch swPar = Stopwatch.StartNew();

            Parallel parallel = new Parallel(LENGTH, EMPTY_ENTRY);
            List<char[,]> solutions = parallel.solveSudoku(boardPar);

            swPar.Stop();

            double timePar = Convert.ToDouble(swPar.ElapsedMilliseconds) / Convert.ToDouble(solutions.Count);

            #endregion

            Console.WriteLine("Sequential time:             " + timeSeq + "ms");
            Console.WriteLine("Parallel time per solution:  " + timePar + "ms");
            Console.WriteLine("Soutions found:              " + solutions.Count);
            Console.WriteLine();
            Console.WriteLine("Original board:");
            printBoard(board);
            Console.WriteLine();
            Console.WriteLine("Sequential board (" + (checkBoard(boardSeq) ? "CORRECT" : "INCORRECT") + "):");
            printBoard(boardSeq);
            //Console.WriteLine();
            //Console.WriteLine("Parallel board: (" + (checkBoard(boardPar) ? "CORRECT" : "INCORRECT") + "):");
            //printBoard(boardPar);

            Console.ReadLine();
        }

        private static bool readTestCase(string path, char[,] board)
        {
            try
            {
                streamReader = new StreamReader(path);
                string line;
                int j = 0;
                while ((line = streamReader.ReadLine()) != null)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        board[j, i] = line[i];
                    }
                    j++;
                }
                streamReader.Close();
                return true;
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        private static void printBoard(char[,] board)
        {
            int length = (int)Math.Sqrt(board.Length);
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    Console.Write(board[i, j] + " ");
                }
                Console.WriteLine();
            }

        }

        private static bool checkBoard(char[,] board)
        {
            double checkSum = (LENGTH + 1) * ((double)LENGTH / 2);

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
