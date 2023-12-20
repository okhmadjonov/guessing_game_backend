using guessing_game_backend.Models;
using System.Reflection;

namespace guessing_game_backend.Repositories
{
    public interface IGameRepository
    {
        Task<int> CreateGame(Game game);
    }
}
