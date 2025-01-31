using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MilestoneProject.Controllers
{
    [Authorize] // Restrict access
    public class GameController : Controller
    {
        [HttpGet]
        public IActionResult StartGame()
        {
            return View();
        }
    }
}
