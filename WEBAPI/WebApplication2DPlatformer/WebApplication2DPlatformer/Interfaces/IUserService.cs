using Microsoft.AspNetCore.Mvc;

namespace WebApplication2DPlatformer.Interfaces
{
    public interface IUserService
    {
        Task<IActionResult> GetUserByLoginAndPasswordAsync(string login, string password);
        Task<IActionResult> CompleteLevelByID(int id, int starsCount, int levelScore, int levelID);
        Task<IActionResult> GetLeaderBoard();
        Task<IActionResult> GetLeaderboardByLevelID(int id);
    }
}