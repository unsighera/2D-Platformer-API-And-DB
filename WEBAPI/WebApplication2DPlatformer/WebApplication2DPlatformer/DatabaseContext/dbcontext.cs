using Microsoft.EntityFrameworkCore;
using WebApplication2DPlatformer.Model;

namespace WebApplication2DPlatformer.DatabaseContext
{
    public class dbcontext : DbContext
    {
        public dbcontext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<LevelProgress> LevelsProgress { get; set; }
    }
}
