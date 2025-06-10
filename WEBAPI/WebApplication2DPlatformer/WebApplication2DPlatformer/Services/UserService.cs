using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2DPlatformer.DatabaseContext;
using WebApplication2DPlatformer.Interfaces;
using WebApplication2DPlatformer.Model;

namespace WebApplication2DPlatformer.Services
{
    public class UserService : IUserService
    {
        private readonly dbcontext _context;

        public UserService(dbcontext context)
        {
            _context = context;
        }

        public async Task<IActionResult> CompleteLevelByID(int id, int starsCount, int levelScore, int levelID)
        {
            // Проверяем, существует ли уже запись для этого пользователя и уровня
            var existingProgress = await _context.LevelsProgress
                .FirstOrDefaultAsync(lp => lp.User_ID == id && lp.Level_ID == levelID);

            if (existingProgress != null)
            {
                // Обновляем существующую запись, если новый результат лучше
                if (levelScore > existingProgress.LevelScore)
                {
                    existingProgress.LevelScore = levelScore;
                    existingProgress.LevelStars = starsCount;
                }
            }
            else
            {
                // Создаем новую запись
                var levelProgress = new LevelProgress
                {
                    User_ID = id,
                    Level_ID = levelID,
                    LevelScore = levelScore,
                    LevelStars = starsCount
                };
                _context.LevelsProgress.Add(levelProgress);
            }

            try
            {
                await _context.SaveChangesAsync();
                return new OkObjectResult(new
                {
                    status = true,
                    message = "Progress saved successfully"
                });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = ex.Message
                });
            }
        }

        public async Task<IActionResult> GetLeaderBoard()
        {
            try
            {
                var leaderboard = await _context.LevelsProgress
                    .Include(lp => lp.User)
                    .Include(lp => lp.Level)
                    .OrderByDescending(lp => lp.LevelScore)
                    .Take(100) // Ограничиваем количество записей
                    .Select(lp => new
                    {
                        Username = lp.User.Login,
                        lp.LevelScore,
                        lp.LevelStars,
                        LevelID = lp.Level_ID,
                        UserID = lp.User_ID
                    })
                    .ToListAsync();

                return new OkObjectResult(new
                {
                    status = true,
                    data = leaderboard
                });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = ex.Message
                });
            }
        }

        public async Task<IActionResult> GetLeaderboardByLevelID(int id)
        {
            try
            {
                var leaderboard = await _context.LevelsProgress
                    .Include(lp => lp.User)
                    .Include(lp => lp.Level)
                    .Where(lp => lp.Level_ID == id)
                    .OrderByDescending(lp => lp.LevelScore)
                    .Take(100)
                    .Select(lp => new
                    {
                        Username = lp.User.Login,
                        lp.LevelScore,
                        lp.LevelStars,
                        LevelID = lp.Level_ID,
                        UserID = lp.User_ID
                    })
                    .ToListAsync();

                return new OkObjectResult(new
                {
                    status = true,
                    data = leaderboard
                });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = ex.Message
                });
            }
        }

        public async Task<IActionResult> GetUserByLoginAndPasswordAsync(string login, string password)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Login == login && u.Password == password);

                if (user == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        status = false,
                        message = "Invalid login or password"
                    });
                }

                return new OkObjectResult(new
                {
                    status = true,
                    data = new
                    {
                        id = user.ID,
                        username = user.Login
                    }
                });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = ex.Message
                });
            }
        }
    }
}