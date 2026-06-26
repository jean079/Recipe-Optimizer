using Microsoft.AspNetCore.Mvc;
using RecipeOptimizer.Application.DTO;
using RecipeOptimizer.Application.Interfaces;
using RecipeOptimizer.Domain.Models;

namespace RecipeOptimizer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeService _recipeService;
        private readonly IInventoryService _inventoryService;
        private readonly IOptimizationService _optimizationService;

        public RecipesController(
            IRecipeService recipeService,
            IInventoryService inventoryService,
            IOptimizationService optimizationService)
        {
            _recipeService = recipeService;
            _inventoryService = inventoryService;
            _optimizationService = optimizationService;
        }

        /// <summary>
        /// Get all available recipes
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<RecipeDto>>> GetAllRecipes()
        {
            var recipes = await _recipeService.GetAllRecipesAsync();
            return Ok(recipes);
        }

        /// <summary>
        /// Get a specific recipe by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<RecipeDto>> GetRecipeById(int id)
        {
            var recipe = await _recipeService.GetRecipeByIdAsync(id);
            if (recipe == null)
                return NotFound();

            return Ok(recipe);
        }
    }
}