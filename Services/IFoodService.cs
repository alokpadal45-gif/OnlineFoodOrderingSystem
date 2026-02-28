using OnlineFoodOrderingSystem.Models;
using OnlineFoodOrderingSystem.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineFoodOrderingSystem.Services
{
    public interface IFoodService
    {
        Task<PaginatedList<Food>> GetPagedAsync(string? search, int pageIndex, int pageSize);
        Task<Food?> GetByIdAsync(int id);
        Task<List<Food>> GetAllAsync();
        Task AddAsync(Food food);
        Task UpdateAsync(Food food);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}