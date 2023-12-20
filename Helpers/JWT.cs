using guessing_game_backend.Dto;
using guessing_game_backend.Models;
using guessing_game_backend.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace guessing_game_backend.Helpers
{
    public class JWT
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        public JWT(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;

        }

        public string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        
    };

            var identity = new ClaimsIdentity(new List<Claim> {new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),}, "custom", ClaimTypes.NameIdentifier, ClaimTypes.Role);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "http://localhost:5123",
                audience: "http://localhost:5123", 
                claims: identity.Claims, 
                expires: DateTime.Now.AddHours(5),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }




        public string Login(LoginDto loginDto)
        {
            if (loginDto.Email != null)
            {
                var user = _userRepository.GetUserByEmail(loginDto.Email);

                var verify = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Result?.Password);
                if (verify)
                {
                    if (user.Result != null) return CreateToken(user.Result);
                }
                return "wrong";
            }

            return "wrong";
        }


        public User Registration(UserDto userDto)
        {
            var byEmail = _userRepository.GetUserByEmail(userDto.Email);
            if (byEmail.Result is null)
            {
                string passwordHash
                    = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

                User user = new User();
                user.Name = userDto.Name;
                user.Password = passwordHash;
                user.Email = userDto.Email;

                _userRepository.CreateUser(user);
                return user;
            }
            else
            {
                return null;
            }
        }

    }
}
