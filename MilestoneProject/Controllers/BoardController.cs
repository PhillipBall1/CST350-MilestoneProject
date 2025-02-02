using Microsoft.AspNetCore.Mvc;
using MilestoneProject.Models;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text.Json;

namespace MilestoneProject.Controllers
{
    public class BoardController : Controller
    {

        private HashSet<(int, int)> visitedTiles = new HashSet<(int, int)>();

        private readonly BoardService boardService;

        private float finalScore;

        private int baseScore = 500;

        public BoardController(BoardService boardService)
        {
            this.boardService = boardService;
        }
        
        [HttpPost] // Reveal Action From View (Using post to send x and y)
        public IActionResult RevealTile(int x, int y)
        {
            // start a timer
            if(boardService.startTime == new DateTime(2000, 1, 1, 2, 3, 4)) boardService.startTime = DateTime.Now;

            // already revealed
            if (boardService.board[x, y].isRevealed) return RedirectToAction("Index");

            // lose condition
            if(boardService.board[x, y].isBomb) return RedirectToAction("Lose");

            // set revealed
            boardService.board[x, y].isRevealed = true;

            // if neighbor bombs = 0, recursively check nearby tiles
            if (boardService.board[x, y].neighborBombs == 0) RevealAdjacentTiles(x, y);

            return RedirectToAction("Index");
        }

        // Recursive Reveal
        private void RevealAdjacentTiles(int row, int column)
        {
            // bounds check
            if (row < 0 || column < 0 || row >= boardService.boardSize || column >= boardService.boardSize) return;

            // prevents revisiting
            if (visitedTiles.Contains((row, column))) return;

            visitedTiles.Add((row, column));

            // skip bombs
            if (boardService.board[row, column].isBomb) return;

            // reveal tile
            boardService.board[row, column].isRevealed = true;

            // stop if neighboring bombs exist
            if (boardService.board[row, column].neighborBombs > 0) return;

            // recursively call other adjacent tiles
            for (int finalX = -1; finalX <= 1; finalX++)
            {
                for (int finalY = -1; finalY <= 1; finalY++)
                {
                    if (finalX == 0 && finalY == 0) continue;

                    int newX = row + finalX;
                    int newY = column + finalY;

                    RevealAdjacentTiles(newX, newY);
                }
            }
        }

        private bool CheckWin()
        {
            for (int row = 0; row < boardService.boardSize; row++)
            {
                for (int column = 0; column < boardService.boardSize; column++)
                {
                    // if tiles are not revealed or not a bomb return false
                    if (!boardService.board[row, column].isRevealed && !boardService.board[row, column].isBomb)
                    {
                        
                        return false;
                    }
                }
            }
            return true;
        }


        public IActionResult Index()
        {
            // win condition
            if (CheckWin())
            {
                // calculate elapsed time
                TimeSpan difference = DateTime.Now - boardService.startTime;

                // get seconds from time difference
                float elapsedTime = (difference.Minutes * 60) + difference.Seconds;

                // calculate final score
                finalScore = ((baseScore * boardService.boardSize * boardService.boardDifficulty) / elapsedTime);
                

                Console.WriteLine($"User won with a score of {finalScore} after {elapsedTime} seconds");
                return RedirectToAction("Win");
            }

            return View(boardService.board);
        }
    }
}
