using MilestoneProject.Models;

public class BoardService
{
    // board matrix
    public TileModel[,] board { get; private set; }

    // size of the board, default 10
    public int boardSize { get; set; } = 10;

    // 1 to 10 range each representing 10% (1 meaning there is a 10% chance for a tile to be a bomb)
    public int boardDifficulty { get; set; } = 1; 

    // time that this board is generated
    public DateTime startTime { get; set; } = new DateTime(2000,1,1,2,3,4);

    public BoardService()
    {
        InitializeBoard();
    }

    private void InitializeBoard()
    {
        board = new TileModel[boardSize, boardSize];

        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                bool isBomb = new Random().Next(10) < boardDifficulty;
                board[x, y] = new TileModel(x, y, isBomb);
            }
        }

        CalculateNeighborBombs();
    }

    // Check Neighbors
    private void CalculateNeighborBombs()
    {
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                // is bomb dont need to calculate neighbors
                if (board[x, y].isBomb) continue;

                board[x, y].neighborBombs = CountAdjacentBombs(x, y);
            }
        }
    }

    // Count Neighbor Bombs
    private int CountAdjacentBombs(int row, int column)
    {
        int count = 0;

        for (int moveX = -1; moveX <= 1; moveX++)
        {
            for (int moveY = -1; moveY <= 1; moveY++)
            {
                // skip self
                if (moveX == 0 && moveY == 0) continue;

                int finalX = row + moveX;
                int finalY = column + moveY;

                // bounds check
                if (!(finalX >= 0 && finalY >= 0 && finalX < boardSize && finalY < boardSize)) continue;

                if (board[finalX, finalY].isBomb) count++;
            }
        }

        return count;
    }

}
