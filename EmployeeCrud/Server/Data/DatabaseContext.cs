using EmployeeCrud.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace EmployeeCrud.Server.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options):base(options) { }

        public DbSet<User> User { get; set; }
        public DbSet<Employee> Employee { get; set; }
    }
}
