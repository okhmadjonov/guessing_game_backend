using guessing_game_backend.Dto;
using guessing_game_backend.Helpers;
using guessing_game_backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace guessing_game_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JWT _jwtService;


        public AuthController(JWT jWT)
        {
            _jwtService = jWT;
        }

        [HttpPost("register")]
        public ActionResult<User> Register(UserDto request)
        {
            User registeredUser = _jwtService.Registration(request);
            if (registeredUser is not null)
            {
                return Ok(registeredUser);
            }
            else
            {
                return BadRequest("User with this email already exists.");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto request)
        {
            try
            {
                return Ok( _jwtService.Login(request));
            }
            catch (Exception e)
            {
                return BadRequest("Incorrect User or Password");
            }
        }


    }
}
