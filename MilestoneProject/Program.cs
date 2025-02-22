using Microsoft.EntityFrameworkCore;
using MilestoneProject.Models;
using MilestoneProject.Service;

namespace MilestoneProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Service Singletons
            builder.Services.AddSingleton<BoardService>();
            builder.Services.AddSingleton<GameService>();

            // Connect to DB
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<DBContext>(options => options.UseSqlServer(connectionString));


            // Set up authentication with cookie
            builder.Services.AddAuthentication("CookieAuth").AddCookie("CookieAuth", options =>
            {
                options.Cookie.Name = "CookieAuth";
                options.LoginPath = "/Account/Login"; // Redirect to Login page
                options.AccessDeniedPath = "/Account/AccessDenied"; // Redirect if access is denied
            });
            builder.Services.AddAuthorization();

            // Add Debug logging
            builder.Logging.AddDebug();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}