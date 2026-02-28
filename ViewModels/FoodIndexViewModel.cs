using OnlineFoodOrderingSystem.Models;
using OnlineFoodOrderingSystem.Helpers;
using System.Collections.Generic;

namespace OnlineFoodOrderingSystem.ViewModels
{
    public class FoodIndexViewModel
    {
        public PaginatedList<Food> Foods { get; set; } = default!;
        public string? SearchString { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}