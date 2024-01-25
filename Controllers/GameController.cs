using guessing_game_backend.Helpers;
using guessing_game_backend.Models;
using guessing_game_backend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace guessing_game_backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IGameRepository _gameRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GameController(IMemoryCache memoryCache, IGameRepository gameRepository, IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _memoryCache = memoryCache;
            _gameRepository = gameRepository;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public IActionResult Get()
        {
            string sessionId = Guid.NewGuid().ToString();
            var gameSession = new GameStart();

            _memoryCache.Set(sessionId, gameSession);
            Game game = new Game();
            game.Attempts = 0;
            _memoryCache.Set("game", game);
            //return Ok($"Welcome, You have 8 attempts. Your session ID is: {sessionId}");
            return Ok(JsonConvert.ToString(sessionId));
        }

        [HttpPost("guess/{sessionId}")]
        public async Task<IActionResult> MakeGuess(string sessionId, string userInput)
        {
            Game game = _memoryCache.Get<Game>("game")!;

            if (_memoryCache.TryGetValue(sessionId, out var gameSession) && gameSession is GameStart session)
            {
                int[] userGuess;
                foreach (int digit in session.SecretNumber)
                {
                    Console.Write(digit + " ");
                }

                if (GameLogic.TryParseInput(userInput, out userGuess))
                {
                    if (userGuess.SequenceEqual(session.SecretNumber))
                    {
                        session.Description.Add(userInput + " => " + "Congratulations! You find the number.");
                        game.Attempts++;
                        _memoryCache.Remove(sessionId);
                        game.Win = true;
                        game.Description = session.Description;
                        int gameId = await _gameRepository.CreateGame(game);
                        var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                        if (int.TryParse(userIdClaim, out int userId))
                        {
                            var user = await _userRepository.GetUserById(userId);

                            if (user != null && user.Id != 0)
                            {
                              
                                if (user.Games == null)
                                {
                                    user.Games = new List<Game>();
                                }
                                user.Games.Add(game);

                                await _userRepository.UpdateUser(user.Id, gameId);
                            }
                            var jsonData = new {status= "Started", message = "Congratulations! You find the number." };
                            return new JsonResult(jsonData);
                        }
                    }
                    else
                    {
                        session.Attempts--;
                        game.Attempts++;
                        int m = GameLogic.CalculateM(session.SecretNumber, userGuess);
                        int p = GameLogic.CalculateP(session.SecretNumber, userGuess);
                        m = m - p;


                        var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);


                        if (int.TryParse(userIdClaim, out int uId))
                        {
                            if (session.Attempts == 0)
                            {
                                session.Description.Add(userInput + " => " + $"You are failed. The secret number was {string.Join("", session.SecretNumber)}.");
                                _memoryCache.Remove(sessionId);
                                game.Win = false;
                                game.Description = session.Description;
                                if (uId != 0)
                               {
                                var user = await _userRepository.GetUserById(uId);

                                if (user != null)
                                    {
                                    
                                    if (user.Games == null)
                                    {
                                        user.Games = new List<Game>();
                                    }

                              
                                    user.Games.Add(game);
                                    int id = await _gameRepository.CreateGame(game);
                                    await _userRepository.UpdateUser(user.Id, id);
                                  }
                                 }
                             
                                var failResult = new { status = "Failed", message = $"You are failed. The secret number was {string.Join("", session.SecretNumber)}."};
                                return new JsonResult(failResult);
                            }
                            session.Description.Add(userInput + " => " + $"Incorrect guess. M: {m}, P: {p}. You have {session.Attempts} attempts left.");
                            var jsonData = new { status = "Started", message = $"Incorrect guess. M: {m}, P: {p}. You have {session.Attempts} attempts left." };
                            return new JsonResult(jsonData);

                        }

                    }   
                }
                else
                {
                    return BadRequest("Invalid input. Please enter a 4-digit number.");
                }
            }
            else
            {
                return NotFound("Invalid or expired session ID.");
            }

           return NotFound("Invalid or expired session ID.");

        }


    }
}

