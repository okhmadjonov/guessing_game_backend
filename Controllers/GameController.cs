﻿using guessing_game_backend.Helpers;
using guessing_game_backend.Models;
using guessing_game_backend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
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
            return Ok($"Welcome to the Number Guessing Game! Guess a number. You have 8 attempts. Your session ID is: {sessionId}");
        }





        [HttpPost("guess/{sessionId}")]
        public async Task<IActionResult> MakeGuess(string sessionId, string userInput)
        {
            Game game = _memoryCache.Get<Game>("game")!;

            if (_memoryCache.TryGetValue(sessionId, out var gameSession) && gameSession is GameStart session)
            {
                int[] userGuess;

                if (GameLogic.TryParseInput(userInput, out userGuess))
                {
                    foreach (int digit in session.SecretNumber)
                    {
                        Console.Write(digit + " ");
                    }

                    if (userGuess.SequenceEqual(session.SecretNumber))
                    {
                        session.Description.Add(userInput + " => " + "Congratulations! You've guessed the number.");
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
                                // Ensure the Games collection is initialized
                                if (user.Games == null)
                                {
                                    user.Games = new List<Game>();
                                }

                                // Associate the game with the user
                                user.Games.Add(game);

                                // Update the user in the repository
                                await _userRepository.UpdateUser(user.Id, gameId);
                            }

                            return Ok("Congratulations! You've guessed the number.");
                        }
                        else
                        {
                            session.Attempts--;
                            game.Attempts++;
                            int m = GameLogic.CalculateM(session.SecretNumber, userGuess);
                            int p = GameLogic.CalculateP(session.SecretNumber, userGuess);
                            m = m - p;

                            int id = await _gameRepository.CreateGame(game);
                            // var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                            int uId;
                            if (int.TryParse(userIdClaim, out uId))
                            {
                                if (userId != 0)
                                {
                                    var user = await _userRepository.GetUserById(userId);

                                    if (user != null)
                                    {
                                        // Ensure the Games collection is initialized
                                        if (user.Games == null)
                                        {
                                            user.Games = new List<Game>();
                                        }

                                        // Associate the game with the user
                                        user.Games.Add(game);

                                        await _userRepository.UpdateUser(user.Id, id);
                                    }
                                }
                            }

                            if (session.Attempts == 0)
                            {
                                session.Description.Add(userInput + " => " + $"Sorry, you didn't guess the number. The secret number was {string.Join("", session.SecretNumber)}.");
                                _memoryCache.Remove(sessionId);
                                game.Win = false;
                                game.Description = session.Description;
                                return Ok($"Sorry, you didn't guess the number. The secret number was {string.Join("", session.SecretNumber)}.");
                            }

                            session.Description.Add(userInput + " => " + $"Incorrect guess. M: {m}, P: {p}. You have {session.Attempts} attempts left.");
                            return Ok($"Incorrect guess. M: {m}, P: {p}. You have {session.Attempts} attempts left.");
                        }
                    }
                }

                // Moved the "Invalid input" return statement here
                return BadRequest("Invalid input. Please enter a 4-digit number.");
            }

            // Add a default return statement if none of the conditions are met
            return NotFound("Invalid or expired session ID.");
        }

    }
}
