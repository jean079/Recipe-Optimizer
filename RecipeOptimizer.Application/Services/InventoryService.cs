using RecipeOptimizer.Application.DTO;
using RecipeOptimizer.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.EntityFrameworkCore;
using RecipeOptimizer.Infrastructure.Data;

namespace RecipeOptimizer.Application.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly AppDbContext _context;

        public InventoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<InventoryDto>> GetInventoryAsync()
        {
            var inventory = await _context.Inventories
                .Include(i => i.Ingredient)
                .ToListAsync();

            return inventory.Select(i => new InventoryDto
            {
                IngredientId = i.IngredientId,
                IngredientName = i.Ingredient?.Name ?? "Unknown",
                AvailableQuantity = i.AvailableQuantity
            }).ToList();
        }
    }
}
