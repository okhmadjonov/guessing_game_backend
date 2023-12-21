using guessing_game_backend.Dto;
using guessing_game_backend.FluentValidation;
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
            // Validate the request
            var validator = new UserDtoValidator();
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(error => error.ErrorMessage));
            }

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
            // Validate the request
            var validator = new LoginDtoValidator();
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(error => error.ErrorMessage));
            }

            try
            {
                return Ok(_jwtService.Login(request));
            }
            catch (Exception e)
            {
                return BadRequest("Incorrect User or Password");
            }
        }



    }
}
