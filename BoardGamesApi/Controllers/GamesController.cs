using BoardGamesApi.Data;
using Microsoft.AspNetCore.Mvc;

namespace BoardGamesApi.Controllers
{
    [Route("api/[controller]")]
    public class GamesController : Controller
    {
        private readonly IGamesRepository _gamesRepository;

        public GamesController(IGamesRepository gamesRepository)
        {
            _gamesRepository = gamesRepository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var games = _gamesRepository.GetAll();

            return Ok(games);
        }
    }
}
