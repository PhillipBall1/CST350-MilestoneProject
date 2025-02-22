using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MilestoneProject.Models;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace MilestoneProject.Service
{
    [ApiController] 
    [Route("Game")]
    public class GameService : ControllerBase
    {
        private string connectionString = "Server=(localdb)\\mssqllocaldb;Database=aspnet-53bc9b9d-9d6a-45d4-8429-2a2761773502;Trusted_Connection=True;MultipleActiveResultSets=true";

        [HttpPost("SaveGame")]
        public async Task<IActionResult> SaveGame([FromBody] GameModel game)
        {
            try
            {
                if (game == null || string.IsNullOrEmpty(game.GameData))
                {
                    return BadRequest("Invalid game data.");
                }

                using (var connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Games (UserEmail, DateSaved, GameData) VALUES (@UserEmail, @DateSaved, @GameData)";

                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserEmail", game.UserEmail);
                        command.Parameters.AddWithValue("@DateSaved", game.DateSaved);
                        command.Parameters.AddWithValue("@GameData", game.GameData);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok(new { message = "Game saved successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while saving the game: {ex.Message}");
            }
        }

        [HttpGet("LoadGame/{id}")]
        public async Task<IActionResult> LoadGame(int id)
        {
            GameModel game = null;

            using (var connection = new SqlConnection(connectionString))
            {
                string query = "SELECT ID, UserEmail, DateSaved, GameData FROM Games WHERE ID = @ID";

                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            game = new GameModel
                            {
                                ID = reader.GetInt32(0),
                                UserEmail = reader.GetString(1),
                                DateSaved = reader.GetDateTime(2),
                                GameData = reader.IsDBNull(3) ? null : reader.GetString(3)
                            };
                        }
                    }
                }
            }

            if (game == null || string.IsNullOrEmpty(game.GameData)) return NotFound(new { message = "Game not found" });

            try
            {
                // directly deserialize GameData into a List<TileModel>
                List<TileModel> tiles = JsonSerializer.Deserialize<List<TileModel>>(game.GameData);

                return Ok(new
                {
                    gameId = game.ID,
                    tiles = tiles
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"JSON Deserialization Error: {ex.Message}");
                return StatusCode(500, $"Error deserializing game data: {ex.Message}");
            }
        }


        [HttpDelete("DeleteGame/{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Games WHERE ID = @ID";

                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", id);
                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected == 0)
                        {
                            return NotFound(new { message = "Game not found" });
                        }
                    }
                }

                return Ok(new { message = "Game deleted successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting game: {ex.Message}");
            }
        }

        [HttpGet("GetGamesForUser/{email}")]
        public async Task<List<GameModel>> GetGamesForUser(string email)
        {
            List<GameModel> userGames = new List<GameModel>();

            using (var connection = new SqlConnection(connectionString))
            {
                string query = "SELECT ID, UserEmail, DateSaved, GameData FROM Games WHERE UserEmail = @Email ORDER BY DateSaved DESC";

                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            userGames.Add(new GameModel
                            {
                                ID = reader.GetInt32(0),
                                UserEmail = reader.GetString(1),
                                DateSaved = reader.GetDateTime(2),
                                GameData = reader.IsDBNull(3) ? null : reader.GetString(3)
                            });
                        }
                    }
                }
            }

            return userGames;
        }

    }
}