using Microsoft.AspNetCore.Mvc;
using WebApplication2DPlatformer.Classes;
using WebApplication2DPlatformer.Interfaces;

namespace WebApplication2DPlatformer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserServiceController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserServiceController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("getUser")]
        public async Task<IActionResult> GetUserByLoginAndPassword([FromBody] UserRequest request)
        {
            return await _userService.GetUserByLoginAndPasswordAsync(request.Login, request.Password);
        }

        [HttpPost("completeLevel")]
        public async Task<IActionResult> CompleteLevelByID([FromBody] CompleteLevelRequest request)
        {
            return await _userService.CompleteLevelByID(request.id, request.starsCount, request.levelScore, request.levelID);
        }

        [HttpGet("getLeaderboard")]
        public async Task<IActionResult> GetLeaderBoard()
        {
            return await _userService.GetLeaderBoard();
        }

        [HttpGet("getLeaderboard/{levelID}")]
        public async Task<IActionResult> GetLeaderboardByLevelID(int levelID)
        {
            return await _userService.GetLeaderboardByLevelID(levelID);
        }
    }
}