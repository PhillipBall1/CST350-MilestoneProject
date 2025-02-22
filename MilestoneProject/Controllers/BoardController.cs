using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Data.SqlClient;
using MilestoneProject.Models;
using MilestoneProject.Service;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;


namespace MilestoneProject.Controllers
{
    public class BoardController : Controller
    {
        private readonly BoardService boardService;
        private readonly GameService gameService;

        private float finalScore;

        private int baseScore = 500;

        public BoardController(BoardService boardService, GameService gameService)
        {
            this.boardService = boardService;
            this.gameService = gameService;
        }

        public IActionResult Index(int gameId = 0, int boardSize = 10, int difficulty = 0)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            ViewData["UserEmail"] = userEmail;

            if(gameId == 0) // fresh Game
            { 
                boardService.SetBoardSizeAndDifficulty(boardSize, difficulty);
            }
            else if (gameId > 0) // game loading from save
            {
                // the IActionResult from LoadGame
                var actionResult = gameService.LoadGame(gameId).Result;
                var okResult = actionResult as OkObjectResult;

                if (okResult == null) return View(boardService.board);

                // extract tiles list
                var response = okResult.Value as dynamic;
                List<TileModel> tiles = response?.tiles;

                foreach(TileModel tile in tiles)
                {
                    Console.WriteLine(tile.row + " | " + tile.column + " | " + tile.neighborBombs);
                }

                if (tiles == null || tiles.Count == 0) return View(boardService.board);

                // load tiles into board
                boardService.LoadBoard(tiles);
            }
            Console.WriteLine("Board Size: " + boardService.boardSize);
            ViewBag.TileSize = (700 / boardService.boardSize).ToString() + "px";
            ViewBag.FontSize = (140 / boardService.boardSize * 2).ToString() + "px";

            return View(boardService.board);
        }



        [HttpPost] // reveals tiles using x and y coords
        public IActionResult RevealTile(int x, int y)
        {
            // if revealed, return NoContent
            if (boardService.board[x, y].isRevealed) return NoContent();

            // lose condition
            if (boardService.board[x, y].isBomb) return Json(new { redirect = Url.Action("Lost") });

            // clear the list before each reveal
            boardService.revealedTiles.Clear();
            boardService.revealedTiles.Add((x, y));

            // reveal tile and adjacent tiles if neighborBombs is 0
            boardService.board[x, y].isRevealed = true;
            if (boardService.board[x, y].neighborBombs == 0) boardService.RevealAdjacentTiles(x, y);

            // check if all of the Tiles are revealed
            if (boardService.CheckWin()) return Json(new { redirect = Url.Action("Won") });

            // return a list of revealed tiles with their current state
            var revealedTileData = boardService.revealedTiles.Select(coord => new
            {
                x = coord.Item1,
                y = coord.Item2,
                neighborBombs = boardService.board[coord.Item1, coord.Item2].neighborBombs
            });

            return Json(revealedTileData);
        }

        // player lost redirect from ajax
        public IActionResult Lost(int elapsedTime)
        {
            ViewBag.ElapsedTime = elapsedTime;
            ViewBag.BoardSize = boardService.boardSize;
            ViewBag.Difficulty = boardService.boardDifficulty;
            boardService.GameEnd();
            return View();
        }

        // player won redirect from ajax
        public IActionResult Won(int elapsedTime)
        {
            if (elapsedTime == 0) elapsedTime = 1;
            // calculate final score using elapsed time
            finalScore = ((baseScore * boardService.boardSize * boardService.boardDifficulty) / elapsedTime);

            // rounded score
            int roundedScore = (int)MathF.Round(finalScore);

            ViewBag.Score = roundedScore;
            ViewBag.Time = elapsedTime;
            ViewBag.BoardSize = boardService.boardSize;
            ViewBag.Difficulty = boardService.boardDifficulty;

            boardService.GameEnd();

            return View();
        }
    }
}
