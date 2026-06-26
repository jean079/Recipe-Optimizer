using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeOptimizer.Application.DTO;
using RecipeOptimizer.Application.Interfaces;
using RecipeOptimizer.Infrastructure.Data;

namespace RecipeOptimizer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OptimizationController : ControllerBase
    {
        private readonly IOptimizationService _optimizationService;
        private readonly IRecipeService _recipeService;
        private readonly IInventoryService _inventoryService;
        private readonly AppDbContext _context;

        public OptimizationController(
            IOptimizationService optimizationService,
            IRecipeService recipeService,
            IInventoryService inventoryService,
            AppDbContext context)
        {
            _optimizationService = optimizationService;
            _recipeService = recipeService;
            _inventoryService = inventoryService;
            _context = context;
        }

        // Get all recipes that can be made with current inventory
        [HttpGet("possible-recipes")]
        public async Task<ActionResult<List<RecipeDto>>> GetPossibleRecipes()
        {
            var possibleRecipes = await _optimizationService.GetPossibleRecipesAsync();
            return Ok(possibleRecipes);
        }

        /// Find the optimal combination of recipes to feed the maximum number of people.
        [HttpGet("optimal-combination")]
        public async Task<ActionResult<MealCombinationDto>> GetOptimalCombination()
        {
            // Get all recipes and current inventory
            var recipes = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                .ToListAsync();

            var inventory = await _context.Inventories.ToListAsync();
            var inventoryDict = inventory.ToDictionary(i => i.IngredientId, i => i.AvailableQuantity);

            // Find optimal combination
            var optimalCombination = _optimizationService.FindOptimalCombination(recipes, inventoryDict);

            return Ok(optimalCombination);
        }
    }
}