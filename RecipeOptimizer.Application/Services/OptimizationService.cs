using RecipeOptimizer.Application.DTO;
using RecipeOptimizer.Application.Interfaces;
using RecipeOptimizer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using RecipeOptimizer.Infrastructure.Data;

namespace RecipeOptimizer.Application.Services
{
    public class OptimizationService : IOptimizationService
    {
        private readonly AppDbContext _context;

        public OptimizationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<RecipeDto>> GetPossibleRecipesAsync()
        {
            var recipes = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .ToListAsync();

            var inventory = await _context.Inventories.ToListAsync();
            var inventoryDict = inventory.ToDictionary(i => i.IngredientId, i => i.AvailableQuantity);

            var possibleRecipes = recipes
                .Where(recipe => CanMakeRecipe(recipe, inventoryDict))
                .ToList();

            return possibleRecipes.Select(MapToDto).ToList();
        }

        public MealCombinationDto FindOptimalCombination(
            List<Recipe> recipes,
            Dictionary<int, int> inventory)
        {
            var bestSolution = new MealCombinationDto();
            var bestPeopleFed = 0;
            var bestSelection = new Dictionary<int, int>();

            // Try all possible combinations using backtracking
            var recipeIds = recipes.Select(r => r.Id).ToList();
            Backtrack(recipes, inventory, recipeIds, 0, new Dictionary<int, int>(),
                      ref bestPeopleFed, ref bestSelection);

            // Build the result
            if (bestSelection.Any())
            {
                var mealPlan = new List<MealPlanDto>();
                int totalPeopleFed = 0;

                foreach (var (recipeId, count) in bestSelection)
                {
                    var recipe = recipes.First(r => r.Id == recipeId);
                    var peopleFed = recipe.Feeds * count;
                    totalPeopleFed += peopleFed;

                    mealPlan.Add(new MealPlanDto
                    {
                        RecipeId = recipeId,
                        RecipeName = recipe.Name,
                        Count = count,
                        PeopleFed = peopleFed
                    });
                }

                bestSolution.TotalPeopleFed = totalPeopleFed;
                bestSolution.MealPlan = mealPlan;
                bestSolution.UnusedIngredients = GetUnusedIngredients(recipes, bestSelection, inventory);
            }

            return bestSolution;
        }

        private void Backtrack(
            List<Recipe> recipes,
            Dictionary<int, int> originalInventory,
            List<int> recipeIds,
            int index,
            Dictionary<int, int> currentSelection,
            ref int bestPeopleFed,
            ref Dictionary<int, int> bestSelection)
        {
            // Calculate people fed with current selection
            int peopleFed = currentSelection.Sum(kvp =>
            {
                var recipe = recipes.First(r => r.Id == kvp.Key);
                return recipe.Feeds * kvp.Value;
            });

            // Update best solution if this is better
            if (peopleFed > bestPeopleFed)
            {
                bestPeopleFed = peopleFed;
                bestSelection = new Dictionary<int, int>(currentSelection);
            }

            // Try adding more of each recipe
            for (int i = index; i < recipeIds.Count; i++)
            {
                var recipeId = recipeIds[i];
                var recipe = recipes.First(r => r.Id == recipeId);

                // Try making this recipe one more time
                var tempInventory = new Dictionary<int, int>(originalInventory);

                // Apply current selection to temp inventory
                foreach (var (selRecipeId, count) in currentSelection)
                {
                    var selRecipe = recipes.First(r => r.Id == selRecipeId);
                    foreach (var ingredient in selRecipe.RecipeIngredients)
                    {
                        if (tempInventory.ContainsKey(ingredient.IngredientId))
                            tempInventory[ingredient.IngredientId] -= ingredient.Quantity * count;
                    }
                }

                // Check if we can make the recipe one more time
                if (CanMakeRecipe(recipe, tempInventory))
                {
                    if (!currentSelection.ContainsKey(recipeId))
                        currentSelection[recipeId] = 0;

                    currentSelection[recipeId]++;

                    // Recurse
                    Backtrack(recipes, originalInventory, recipeIds, i, currentSelection,
                              ref bestPeopleFed, ref bestSelection);

                    currentSelection[recipeId]--;
                    if (currentSelection[recipeId] == 0)
                        currentSelection.Remove(recipeId);
                }
            }
        }

        private bool CanMakeRecipe(Recipe recipe, Dictionary<int, int> inventory)
        {
            foreach (var ingredient in recipe.RecipeIngredients)
            {
                if (!inventory.ContainsKey(ingredient.IngredientId) ||
                    inventory[ingredient.IngredientId] < ingredient.Quantity)
                {
                    return false;
                }
            }
            return true;
        }

        private List<UnusedIngredientDto> GetUnusedIngredients(
            List<Recipe> recipes,
            Dictionary<int, int> selection,
            Dictionary<int, int> originalInventory)
        {
            var remaining = new Dictionary<int, int>(originalInventory);

            foreach (var (recipeId, count) in selection)
            {
                var recipe = recipes.First(r => r.Id == recipeId);
                foreach (var ingredient in recipe.RecipeIngredients)
                {
                    remaining[ingredient.IngredientId] -= ingredient.Quantity * count;
                }
            }

            var ingredientNames = _context.Ingredients.ToDictionary(i => i.Id, i => i.Name);

            return remaining
                .Where(kvp => kvp.Value > 0)
                .Select(kvp => new UnusedIngredientDto
                {
                    IngredientName = ingredientNames.ContainsKey(kvp.Key)
                        ? ingredientNames[kvp.Key]
                        : "Unknown",
                    Remaining = kvp.Value
                })
                .ToList();
        }

        private RecipeDto MapToDto(Recipe recipe)
        {
            return new RecipeDto
            {
                Id = recipe.Id,
                Name = recipe.Name,
                Feeds = recipe.Feeds,
                Ingredients = recipe.RecipeIngredients.Select(ri => new RecipeIngredientDto
                {
                    IngredientId = ri.IngredientId,
                    IngredientName = ri.Ingredient?.Name ?? "Unknown",
                    Quantity = ri.Quantity
                }).ToList()
            };
        }
    }
}
