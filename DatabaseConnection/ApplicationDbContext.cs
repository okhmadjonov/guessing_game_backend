using guessing_game_backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace guessing_game_backend.DatabaseConnection
{
    public class ApplicationDbContext : DbContext
    {


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options) {
        
        }


        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }


    
    }
}
