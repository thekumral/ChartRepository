using ChartProject.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ChartProject.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ChartData> ChartData { get; set; }
    }
}
