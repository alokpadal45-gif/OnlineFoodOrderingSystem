using Microsoft.EntityFrameworkCore;
using OnlineFoodOrderingSystem.Data;
using OnlineFoodOrderingSystem.Helpers;
using OnlineFoodOrderingSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineFoodOrderingSystem.Services
{
    public class FoodService : IFoodService
    {
        private readonly ApplicationDbContext _context;

        public FoodService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Food food)
        {
            _context.Foods.Add(food);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var food = await _context.Foods.FindAsync(id);
            if (food != null)
            {
                _context.Foods.Remove(food);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Foods.AnyAsync(f => f.FoodId == id);
        }

        public async Task<List<Food>> GetAllAsync()
        {
            return await _context.Foods.ToListAsync();
        }

        public async Task<Food?> GetByIdAsync(int id)
        {
            return await _context.Foods.FirstOrDefaultAsync(f => f.FoodId == id);
        }

        public async Task<PaginatedList<Food>> GetPagedAsync(string? search, int pageIndex, int pageSize)
        {
            var query = _context.Foods.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(f => f.Name.Contains(search) ||
                                          f.Category.Contains(search));
            }

            query = query.OrderBy(f => f.FoodId);

            return await PaginatedList<Food>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task UpdateAsync(Food food)
        {
            _context.Foods.Update(food);
            await _context.SaveChangesAsync();
        }
    }
}