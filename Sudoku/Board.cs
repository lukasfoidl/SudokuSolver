namespace Sudoku
{
    class Board
    {
        public char[,] grid = new char[9, 9];
        //step = number of solved cells
        public int step;

        public Board(char[,] grid, int step)
        {
            this.grid = grid;
            this.step = step;
        }
    }
}
