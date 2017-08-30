using BoardGamesApi.Data;
using BoardGamesApi.Models;
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

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            _gamesRepository.Delete(id);

            return Ok();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var games = _gamesRepository.GetAll();

            return Ok(games);
        }

        [HttpPost]
        public IActionResult Post(GameInput model)
        {
            var game = new Game();
            model.MapToGame(game);

            _gamesRepository.Create(game);

            var url = $"{Request.Scheme}://{Request.Host}/api/games/{game.Id}";

            return Created(url, game);
        }

        [HttpPut("{id}")]
        public IActionResult Put(string id, GameInput model)
        {
            var game = _gamesRepository.GetById(id);

            model.MapToGame(game);

            _gamesRepository.Update(game);

            return Ok(game);
        }
    }
}
