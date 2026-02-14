using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineFoodOrderingSystem.Data;
using OnlineFoodOrderingSystem.Models;
using System.Threading.Tasks;

namespace OnlineFoodOrderingSystem.Controllers
{
	public class FoodController : Controller
	{
		private readonly ApplicationDbContext _context;

		public FoodController(ApplicationDbContext context)
		{
			_context = context;
		}

		// GET: / or /Food
		public async Task<IActionResult> Index()
		{
			var foods = await _context.Foods.ToListAsync();
			return View(foods);
		}

		// GET: /Food/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var food = await _context.Foods.FirstOrDefaultAsync(f => f.FoodId == id);
			if (food == null)
			{
				return NotFound();
			}

			return View(food);
		}

		// GET: /Food/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: /Food/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("FoodId,FoodName,Description,Price,Category,IsAvailable")] Food food)
		{
			if (ModelState.IsValid)
			{
				_context.Add(food);
				await _context.SaveChangesAsync();
				TempData["SuccessMessage"] = "Food item created successfully!";
				return RedirectToAction(nameof(Index));
			}
			return View(food);
		}

		// GET: /Food/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var food = await _context.Foods.FindAsync(id);
			if (food == null)
			{
				return NotFound();
			}
			return View(food);
		}

		// POST: /Food/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("FoodId,FoodName,Description,Price,Category,IsAvailable")] Food food)
		{
			if (id != food.FoodId)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(food);
					await _context.SaveChangesAsync();
					TempData["SuccessMessage"] = "Food item updated successfully!";
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!FoodExists(food.FoodId))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}
			return View(food);
		}

		// GET: /Food/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var food = await _context.Foods.FirstOrDefaultAsync(f => f.FoodId == id);
			if (food == null)
			{
				return NotFound();
			}

			return View(food);
		}

		// POST: /Food/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var food = await _context.Foods.FindAsync(id);
			if (food != null)
			{
				_context.Foods.Remove(food);
				await _context.SaveChangesAsync();
				TempData["SuccessMessage"] = "Food item deleted successfully!";
			}
			return RedirectToAction(nameof(Index));
		}

		private bool FoodExists(int id)
		{
			return _context.Foods.Any(e => e.FoodId == id);
		}
	}
}
