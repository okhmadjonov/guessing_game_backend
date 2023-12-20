using Microsoft.AspNetCore.Identity;

namespace guessing_game_backend.Models
{
    public class User
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<Game> Games { get; set; }

    }
}
