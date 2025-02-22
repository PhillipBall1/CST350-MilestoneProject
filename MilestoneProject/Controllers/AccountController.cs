using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MilestoneProject.Models;
using MilestoneProject.Service;
using System.Security.Claims;

namespace MilestoneProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly DBContext context;
        private readonly GameService gameService;

        public AccountController(DBContext context, GameService gameService)
        {
            this.context = context;
            this.gameService = gameService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = context.users.FirstOrDefault(u => u.email == model.email);

                if (user != null && user.password == model.password)
                {
                    // Create claims for user
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.username),
                        new Claim(ClaimTypes.Email, user.email),
                        new Claim(ClaimTypes.Role, "User")
                    };

                    var identity = new ClaimsIdentity(claims, "CookieAuth");
                    var principal = new ClaimsPrincipal(identity);

                    // Sign in the user with cookie authentication
                    await HttpContext.SignInAsync("CookieAuth", principal);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid email or password.");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            // Sign out the user and clear the authentication cookie
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Login", "Account"); // Redirect to the login page
        }

        [HttpGet]
        public async Task<IActionResult> SavedGames()
        {
            // get the logged-in user's email
            string? userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            // fetch the user's saved games from GameService
            var savedGames = await gameService.GetGamesForUser(userEmail);

            return View(savedGames); // pass the games list to the view
        }
    }
}
