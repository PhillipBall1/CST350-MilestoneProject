using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilestoneProject.Models;
using System.Linq;

namespace MilestoneProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly DBContext _context;

        public AccountController(DBContext dbContext)
        {
            _context = dbContext;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.users.FirstOrDefault(u => u.email == model.email);

                if (user != null && user.password == model.password)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid email or password.");
            }

            return View(model);
        }
    }
}
