using BoardGamesApi.Data;
using BoardGamesApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BoardGamesApi.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Games endpoint of Board Games API.
    /// </summary>
    [Authorize]
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

        /// <summary>
        /// Delete the game with the given id.
        /// </summary>
        /// <param name="id">Id of the game to delete.</param>
        /// <response code="200">Game successfully deleted</response>
        /// <response code="404">Game with the given ID not found.</response>
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 404)]
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

        /// <summary>
        /// Get one page of games.
        /// </summary>
        /// <param name="page">Page number.</param>
        /// <param name="size">Page size.</param>
        /// <remarks>If you omit <c>page</c> and <c>size</c> query parameters, you'll get the first page with 10 games.</remarks>
        /// <response code="200">Returns a page of games.</response>
        /// <response code="404">Game with the given id not found.</response>
        [HttpGet]
        [ProducesResponseType(typeof(PagedApiResult<Game>), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult GetAll(int page = 1, int size = 10)
        {
            _logger.LogDebug("Getting one page of games");

            var games = _gamesRepository.GetPage(page, size);

            return Ok(games.WrapPagedList());
        }

        /// <summary>
        /// Get a single game by id.
        /// </summary>
        /// <param name="id">Id of the game to retrieve.</param>
        /// <response code="200">Returns the game with the given id.</response>
        /// <response code="404">Game with the given id not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<Game>), 200)]
        [ProducesResponseType(typeof(void), 404)]
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

        /// <summary>
        /// Create a new game from the supplied data.
        /// </summary>
        /// <param name="model">Data to create the game from.</param>
        /// <response code="200">Returns the created game.</response>
        /// <response code="400">Supplied data is not valid.</response>
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<Game>), 200)]
        [ProducesResponseType(typeof(void), 400)]
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

        /// <summary>
        /// Updates the game with the given id.
        /// </summary>
        /// <param name="id">Id of the game to update.</param>
        /// <param name="model">Data to update the game from.</param>
        /// <response code="200">Returns the updated game.</response>
        /// <response code="400">Supplied data is not valid.</response>
        /// <response code="404">Game with the given id not found.</response>
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResult<Game>), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 404)]
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
