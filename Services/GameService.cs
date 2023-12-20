using guessing_game_backend.DatabaseConnection;
using guessing_game_backend.Models;
using guessing_game_backend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace guessing_game_backend.Services
{
    public class GameService : IGameRepository
    {
        private readonly ApplicationDbContext _context;

        public GameService(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }
        public async Task<int> CreateGame(Game game)
        {
        
            await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();

            int gameId = game.Id;

            return gameId;
        }


    }
}
