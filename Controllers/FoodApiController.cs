using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineFoodOrderingSystem.Data;
using OnlineFoodOrderingSystem.Models;

namespace OnlineFoodOrderingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoodApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FoodApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/FoodApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Food>>> GetAllFoods()
        {
            var foods = await _context.Foods.ToListAsync();
            return Ok(foods);
        }

        // GET: api/FoodApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Food>> GetFoodById(int id)
        {
            var food = await _context.Foods.FindAsync(id);
            if (food == null)
                return NotFound(new { message = $"Food ID {id} not found." });
            return Ok(food);
        }

        // POST: api/FoodApi
        [HttpPost]
        public async Task<ActionResult<Food>> CreateFood([FromBody] Food food)
        {
            _context.Foods.Add(food);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetFoodById), new { id = food.FoodId }, food);
        }

        // PUT: api/FoodApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFood(int id, [FromBody] Food food)
        {
            if (id != food.FoodId)
                return BadRequest(new { message = "ID mismatch." });

            _context.Entry(food).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Foods.Any(f => f.FoodId == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/FoodApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFood(int id)
        {
            var food = await _context.Foods.FindAsync(id);
            if (food == null)
                return NotFound(new { message = $"Food ID {id} not found." });

            _context.Foods.Remove(food);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}