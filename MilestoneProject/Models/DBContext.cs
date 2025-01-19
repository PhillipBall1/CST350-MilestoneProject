using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;


namespace MilestoneProject.Models
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options) { }
        public DbSet<UserRegistration> users { get; set; }
    }
}
