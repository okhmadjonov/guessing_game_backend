using guessing_game_backend.DatabaseConnection;
using guessing_game_backend.Models;
using guessing_game_backend.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace guessing_game_backend.Services
{
    public class UserService : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public UserService(ApplicationDbContext applicationDbContext, IHttpContextAccessor contextAccessor)
        {
            _context = applicationDbContext;
            _contextAccessor = contextAccessor;

        }
        public async Task CreateUser(User user)
        {
           _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<List<User>> GetLeaderBoard()
        {
            var users = await _context.Users
                .Where(u => u.Games.Any(g => g.Win)) 
                .Include(u => u.Games.Where(g => g.Win))
                .OrderByDescending(u => u.Games.Count(g => g.Win))
                .ThenBy(u => u.Games.Where(g => g.Win).Min(g => g.Attempts))
                .ToListAsync();

            return users;
        }


        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetUserById(int id, bool includeGames = false)
        {
            var query = _context.Users.AsQueryable();

            if (includeGames)
            {
                query = query.Include(u => u.Games);
            }

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }



        public async Task UpdateUser(int userId, int gameId)
        {
            var user = await _context.Users.Include(u => u.Games).FirstOrDefaultAsync(u => u.Id == userId);

            if (user != null)
            {
                if (user.Games == null)
                {
                    user.Games = new List<Game>();
                }

                var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == gameId);

                if (game != null)
                {
                    user.Games.Add(game);

                    _context.Entry(user).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else if (game == null)
                {
                    throw new ArgumentException($"Game with Id {gameId} not found.");
                }
                else
                {
                    throw new InvalidOperationException($"Provided game with Id {gameId} does not belong to user with Id {userId}.");
                }
            }
            else
            {
                throw new ArgumentException($"User with Id {userId} not found.");
            }
        }



    }
}
