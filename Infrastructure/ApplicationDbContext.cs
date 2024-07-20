using Microsoft.EntityFrameworkCore;
using MonsterGame.Infrastructure.Models;

namespace MonsterGame.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext()
        {
            
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            :base(options)
        {
            
        }

        public DbSet<Champ> Champions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=.;Database=MonsterGameDb;Trusted_Connection=True;TrustServerCertificate=true;");

            base.OnConfiguring(optionsBuilder);
        }
    }
}
