using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SuperHeroAPI.Data;
using System.Xml.Linq;

namespace SuperHeroAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperHeroController : ControllerBase
    {
        private readonly DataContext context;

        public SuperHeroController(DataContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<SuperHero>>> Get()
        {
            
            return Ok(await context.SuperHeroes.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SuperHero>> Getid(int id)
        {
            var dbhero = await context.SuperHeroes.FindAsync(id);
            if(dbhero == null)
            {
                return BadRequest("Hero is not found");
            }
            else
            {
            return Ok(dbhero);
            }
        }

        [HttpPost]
        public async Task<ActionResult<List<SuperHero>>> Post(SuperHero dbhero)
        {
            context.SuperHeroes.Add(dbhero);
            await context.SaveChangesAsync();

            return Ok(await context.SuperHeroes.ToListAsync());
        }
 
        [HttpPut]
        public async Task<ActionResult<List<SuperHero>>> Update(SuperHero request_hero)
        {
            var dbhero = await context.SuperHeroes.FindAsync(request_hero.Id);
            if (dbhero == null)
                return BadRequest("Hero not found");
            dbhero.Name = request_hero.Name;
            dbhero.FirstName = request_hero.FirstName;
            dbhero.LastName = request_hero.LastName;
            dbhero.Place = request_hero.Place;
            await context.SaveChangesAsync();
                    return Ok(await context.SuperHeroes.ToListAsync());
        }

        [HttpDelete]
        public async Task<ActionResult<List<SuperHero>>> Delete(int id)
        {
            var dbhero = await context.SuperHeroes.FindAsync(id);
            if (dbhero == null)
                return BadRequest("Hero not Found");
            context.SuperHeroes.Remove(dbhero);
            await context.SaveChangesAsync();
            return Ok(await context.SuperHeroes.ToListAsync());
        }
    }
}


