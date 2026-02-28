using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineFoodOrderingSystem.Data;
using OnlineFoodOrderingSystem.Models;
using System.Threading.Tasks;

namespace OnlineFoodOrderingSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FoodController : Controller
    {
        private readonly Services.IFoodService _foodService;

        public FoodController(Services.IFoodService foodService)
        {
            _foodService = foodService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string? searchString, int page = 1)
        {
            // small page size; could be configurable
            const int pageSize = 10;
            var model = new ViewModels.FoodIndexViewModel
            {
                SearchString = searchString,
                PageIndex = page,
                PageSize = pageSize,
                Foods = await _foodService.GetPagedAsync(searchString, page, pageSize)
            };
            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var food = await _foodService.GetByIdAsync(id.Value);
            if (food == null) return NotFound();
            return View(food);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FoodId,Name,Description,Price,Category,IsAvailable")] Food food)
        {
            if (ModelState.IsValid)
            {
                await _foodService.AddAsync(food);
                TempData["SuccessMessage"] = "Food item created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(food);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var food = await _foodService.GetByIdAsync(id.Value);
            if (food == null) return NotFound();
            return View(food);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FoodId,Name,Description,Price,Category,IsAvailable")] Food food)
        {
            if (id != food.FoodId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    await _foodService.UpdateAsync(food);
                    TempData["SuccessMessage"] = "Food item updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _foodService.ExistsAsync(food.FoodId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(food);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var food = await _foodService.GetByIdAsync(id.Value);
            if (food == null) return NotFound();
            return View(food);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _foodService.DeleteAsync(id);
            TempData["SuccessMessage"] = "Food item deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> FoodExists(int id)
        {
            return await _foodService.ExistsAsync(id);
        }
    }
}