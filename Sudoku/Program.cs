using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Sudoku
{
    static class Program
    {
        static StreamReader streamReader;
        static string path = @"..\..\Sudoku\";

        [STAThread]
        static void Main()
        {
            char[,] board = new char[9, 9];
            string test_case_name = "case17.txt";
            // read test case from file
            readTestCase(path + test_case_name, board);

            #region Sequential
            // start stop watch to calculate time
            Stopwatch stopwatch = Stopwatch.StartNew();

            Sequential sequential = new Sequential(9);
            sequential.solveSudoku(board);

            stopwatch.Stop();

            double time_seq_sol = Convert.ToDouble(stopwatch.ElapsedMilliseconds);
            #endregion

            #region Parallel
            Stopwatch stopwatch2 = Stopwatch.StartNew();

            Parallel parallel = new Parallel(9);
            List<char[,]> solutions = parallel.solveSudoku(board, 0, 0);

            stopwatch2.Stop();
            // get time per each solution
            double time_per_sol = Convert.ToDouble(stopwatch.ElapsedMilliseconds) / Convert.ToDouble(solutions.Count);

            #endregion
        }

        public static bool readTestCase(String path, char[,] board)
        {
            try
            {
                streamReader = new StreamReader(path);
                String line;
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

        public static void printBoard(char[,] board)
        {
            int length = (int)Math.Sqrt(board.Length);
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                    Console.Write(board[i, j] + " ");
                Console.WriteLine();
            }

        }
    }

}
