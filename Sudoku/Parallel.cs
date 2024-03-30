using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sudoku
{
    class Parallel
    {

        private int length;
        private char emptyEntry;
        private List<Task<char[,]>> taskList = new List<Task<char[,]>>();

        public Parallel(int length, char emptyEntry)
        {
            this.length = length;
            this.emptyEntry = emptyEntry;
        }

        public void solveSudoku(char[,] board)
        {
            // creat thread to start solve suduko
            Task<char[,]> parent = new Task<char[,]>(() =>
            {
                return canSolveSudokuFromCell(0, 0, board);

            });
            parent.Start();
            parent.Wait();

            // get number of empty cell in grid to know how many step should threads to solve 
            int no_empty_cells = getNumOfEmptyCell(board);

            // loop over list of tasks to get successful tasks to solve suduku 
            for (int i = 0; i < taskList.Count; i++)
            {

                try
                {
                    // check number of solved cell for each task to get successful tasks
                    if (taskList[i].Result.step == no_empty_cells)
                    {
                        // put successful task.grid in a list
                        solutions.Add(taskList[i].Result.grid);
                    }
                }
                catch (Exception e)
                {

                }

            }

            // return a list of solutions
            return solutions;
        }

        private char[,] canSolveSudokuFromCell(int row, int col, char[,] board)
        {
            // if reached end of column => take next row, if reached end of rows => finished
            if (col == length)
            {
                col = 0;
                row++;

                if (row == length)
                {
                    return board;
                }
            }

            // if cell is not empty skip it 
            if (board[row, col] != emptyEntry)
            {
                return canSolveSudokuFromCell(row, col + 1, board);
            }

            // for each possible value (1-9) try to place it, if successful => rerun recursive with next cell
            for (int value = 1; value <= length; value++)
            {
                char charToPlace = (char)(value + '0');


                if (canPlaceValue(board, row, col, charToPlace))
                {
                    board[row, col] = charToPlace;

                    // create a copy of grid
                    var boardCopy = board.Clone() as char[,];

                    // create anthor task with the copied board
                    Task<char[,]> t = new Task<char[,]>(() =>
                    {
                        return canSolveSudokuFromCell(row, col + 1, boardCopy);

                    });

                    t.Start();
                    taskList.Add(t);
                }
            }

            return board;
        }

        private bool canPlaceValue(char[,] board, int row, int col, char charToPlace)
        {
            // check column of the placement
            for (int i = 0; i < length; i++)
            {
                if (charToPlace == board[i, col])
                {
                    return false;
                }
            }

            // check row of the placement
            for (int i = 0; i < length; i++)
            {
                if (charToPlace == board[row, i])
                {
                    return false;
                }
            }

            // check sub box of the placement
            int regionSize = (int)Math.Sqrt(length);

            int verticalBoxIndex = row / regionSize;
            int horizontalBoxIndex = col / regionSize;

            int topLeftOfSubBoxRow = regionSize * verticalBoxIndex;
            int topLeftOfSubBoxCol = regionSize * horizontalBoxIndex;

            for (int i = 0; i < regionSize; i++)
            {
                for (int j = 0; j < regionSize; j++)
                {
                    if (charToPlace == board[topLeftOfSubBoxRow + i, topLeftOfSubBoxCol + j])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private int getNumOfEmptyCell(char[,] grid)
        {
            int count = 0;
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (grid[row, col] == emptyEntry)
                        count++;
                }
            }
            return count;
        }
    }
}
