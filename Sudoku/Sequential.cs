using System;

namespace Sudoku
{
    class Sequential
    {
        private int length;
        private char emptyEntry;

        public Sequential(int length, char emptyEntry)
        {
            this.length = length;
            this.emptyEntry = emptyEntry;
        }

        public void solveSudoku(char[,] board)
        {
            canSolveSudokuFromCell(0, 0, board);
        }

        private bool canSolveSudokuFromCell(int row, int col, char[,] board)
        {
            // if reached end of column => take next row, if reached end of rows => finished
            if (col == length)
            {
                col = 0;
                row++;

                if (row == length)
                {
                    return true;
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
                    if (canSolveSudokuFromCell(row, col + 1, board))
                    {
                        return true;
                    }
                    board[row, col] = emptyEntry;
                }
            }

            return false;
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
    }
}
