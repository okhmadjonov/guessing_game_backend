using guessing_game_backend.Models;

namespace guessing_game_backend.Repositories
{
    public interface IUserRepository
    {
        Task CreateUser(User user);

        Task<User> GetUserByEmail(string email);

        Task<User> GetUserById(int id, bool includeGames = false);

        Task UpdateUser(int userId, int gameId);

        Task<List<User>> GetLeaderBoard();
    }
}
