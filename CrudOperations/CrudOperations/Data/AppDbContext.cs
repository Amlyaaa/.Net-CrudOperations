using CrudOperations.Models;
using Microsoft.EntityFrameworkCore;

namespace CrudOperations.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> users {  get; set; }
    }
}
