using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BoardGamesApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TempController : Controller
    {
        private readonly IConfiguration _configuration;

        public TempController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [AllowAnonymous]
        [Route("/get-token")]
        public IActionResult GenerateToken(string name = "mscommunity")
        {
            var jwt = JwtTokenGenerator
                .Generate(name, true, _configuration["Token:Issuer"], _configuration["Token:Key"]);

            return Ok(jwt);
        }
    }
}
