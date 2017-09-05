using BoardGamesApi.Data;
using BoardGamesApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BoardGamesApi.Controllers
{
    [Route("api/[controller]")]
    public class GamesController : Controller
    {
        private readonly IGamesRepository _gamesRepository;
        private readonly ILogger<GamesController> _logger;

        public GamesController(IGamesRepository gamesRepository, ILogger<GamesController> logger)
        {
            _gamesRepository = gamesRepository;
            _logger = logger;
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            _logger.LogDebug($"Deleting game with id {id}");

            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var game = _gamesRepository.GetById(id);
            if (game == null)
                return NotFound();

            _gamesRepository.Delete(id);

            return Ok();
        }

        [HttpGet]
        public IActionResult GetAll(int page = 1, int size = 10)
        {
            _logger.LogDebug("Getting one page of games");

            var games = _gamesRepository.GetPage(page, size);

            return Ok(games.WrapPagedList());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            _logger.LogDebug($"Getting a game with id {id}");

            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var game = _gamesRepository.GetById(id);

            if (game == null)
                return NotFound();

            return Ok(game.WrapData());
        }

        [HttpPost]
        public IActionResult Post([FromBody] GameInput model)
        {
            _logger.LogDebug($"Creating a new game with title \"{model.Title}\"");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var game = new Game();
            model.MapToGame(game);

            _gamesRepository.Create(game);

            var url = $"{Request.Scheme}://{Request.Host}/api/games/{game.Id}";

            return Created(url, game.WrapData());
        }

        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] GameInput model)
        {
            _logger.LogDebug($"Updating a game with id {id}");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var game = _gamesRepository.GetById(id);

            if (game == null)
                return NotFound();

            model.MapToGame(game);

            _gamesRepository.Update(game);

            return Ok(game.WrapData());
        }
    }
}
