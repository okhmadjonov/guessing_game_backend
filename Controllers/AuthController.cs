using guessing_game_backend.Dto;
using guessing_game_backend.FluentValidation;
using guessing_game_backend.Helpers;
using guessing_game_backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
            var validator = new LoginDtoValidator();
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(error => error.ErrorMessage));
            }
            else
            {
                try
                {
                    var token = _jwtService.Login(request);
                    if (token == null)
                    {
                        return Unauthorized("Incorrect User or Password");
                    }
                    else
                    {
                        if(token == "User not found")
                        {
                            string[] myArr = new string[] { "User not found" }; 
                            return BadRequest(new { errors = new { Warning = myArr}});
                        }

                        if (token == "Incorrect password")
                        {
                            string[] myArr = new string[] { "Incorrect password" };
                            return BadRequest(new { errors = new { Warning = myArr } });
                        }

                        return Ok(new { token, email = request.Email });
                    }

                }
                catch (Exception e)
                {
                    return StatusCode(500, "Internal server error");
                }
            }

         
        }

    }
}
