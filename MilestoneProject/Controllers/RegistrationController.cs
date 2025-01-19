using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilestoneProject.Models;
using System.Data.Entity;

namespace MilestoneProject.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly DBContext _context;

        public RegistrationController(DBContext context)
        {
            _context = context;
        }

        // GET: Registration Form
        public IActionResult Index()
        {
            return View();
        }

        // POST: Submit Registration Form
        [HttpPost]
        public IActionResult Submit(UserRegistration model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Save data to the database
                    _context.users.Add(model);
                    _context.SaveChanges();
                    // Redirect to success page
                    return RedirectToAction("Success");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                    Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                    return RedirectToAction("Error");
                }
            }
            // If validation fails, reload the form with errors
            return View("Index", model);
        }

        public IActionResult Success()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
