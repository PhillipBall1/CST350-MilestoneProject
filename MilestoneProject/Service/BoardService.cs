using MilestoneProject.Models;

public class BoardService
{
    // board matrix
    public TileModel[,] board { get; private set; }

    // size of the board, default 10
    public int boardSize { get; set; } = 10;

    // 1 to 100 range each representing 1% (10 meaning there is a 10% chance for a tile to be a bomb)
    public int boardDifficulty { get; set; } = 10;

    // stores visited tiles so we dont recursively stack overflow
    private HashSet<(int, int)> visitedTiles = new HashSet<(int, int)>();

    // stores actively revealed tiles to send to ajax
    public List<(int, int)> revealedTiles = new List<(int, int)>();

    public void SetBoardSizeAndDifficulty(int size, int difficulty)
    {
        boardSize = size;

        switch (difficulty)
        {
            case 0: boardDifficulty = 7; break;
            case 1: boardDifficulty = 15; break;
            case 2: boardDifficulty = 20; break;
        }

        InitializeBoard();
    }

    private void InitializeBoard()
    {
        board = new TileModel[boardSize, boardSize];

        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                bool isBomb = new Random().Next(100) <= boardDifficulty;
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

    // checks if player revealed all available tiles
    public bool CheckWin()
    {
        for (int row = 0; row < boardSize; row++)
        {
            for (int column = 0; column < boardSize; column++)
            {
                // if tiles are not revealed or not a bomb return false
                if (!board[row, column].isRevealed && !board[row, column].isBomb)
                {
                    return false;
                }
            }
        }
        return true;
    }

    // recursive Reveal
    public void RevealAdjacentTiles(int row, int column)
    {
        // bounds check
        if (row < 0 || column < 0 || row >= boardSize || column >= boardSize) return;

        // prevents revisiting
        if (visitedTiles.Contains((row, column))) return;

        visitedTiles.Add((row, column));

        // skip bombs
        if (board[row, column].isBomb) return;

        // reveal tile
        board[row, column].isRevealed = true;

        // track for updating the view
        revealedTiles.Add((row, column));

        // stop if neighboring bombs exist
        if (board[row, column].neighborBombs > 0) return;

        // recursively call other adjacent tiles
        for (int moveX = -1; moveX <= 1; moveX++)
        {
            for (int moveY = -1; moveY <= 1; moveY++)
            {
                if (moveX == 0 && moveY == 0) continue;

                int finalX = row + moveX;
                int finalY = column + moveY;

                RevealAdjacentTiles(finalX, finalY);
            }
        }
    }

    // reset start time and do some other stuff here later
    public void GameEnd()
    {
        visitedTiles.Clear();
        revealedTiles.Clear();
        board = new TileModel[boardSize, boardSize];
    }
}
