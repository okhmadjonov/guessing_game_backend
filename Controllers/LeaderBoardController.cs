using guessing_game_backend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace guessing_game_backend.Controllers
{

    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LeaderBoardController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public LeaderBoardController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
           
            return Ok(await _userRepository.GetLeaderBoard());
        }
    }
}
