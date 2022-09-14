using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SuperHeroAPI.Data;

namespace SuperHeroAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly DataContext country_Context;

        public CountryController(DataContext country_context)
        {
            country_Context = country_context;
        }
        [HttpGet]
        public async Task<ActionResult<List<Countries>>> GetAllCountries()
        {
            return Ok(await country_Context.countries.ToListAsync());
        }
    }
}
