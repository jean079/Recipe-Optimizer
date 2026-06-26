using RecipeOptimizer.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeOptimizer.Application.Interfaces
{
    public interface IRecipeService
    {
        Task<List<RecipeDto>> GetAllRecipesAsync();
        Task<RecipeDto?> GetRecipeByIdAsync(int id);
    }

}
