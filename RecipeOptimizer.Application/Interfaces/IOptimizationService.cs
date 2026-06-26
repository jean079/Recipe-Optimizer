using RecipeOptimizer.Application.DTO;
using RecipeOptimizer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeOptimizer.Application.Interfaces
{
    public interface IOptimizationService
    {
        MealCombinationDto FindOptimalCombination( List<Recipe> recipes, Dictionary<int, int> inventory);
        Task<List<RecipeDto>> GetPossibleRecipesAsync();
    }

}
