using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeOptimizer.Application.DTO
{
    public class MealCombinationDto
    {
        public int TotalPeopleFed { get; set; }
        public List<MealPlanDto> MealPlan { get; set; } = new();
        public List<UnusedIngredientDto> UnusedIngredients { get; set; } = new();
    }

    public class MealPlanDto
    {
        public int RecipeId { get; set; }
        public string RecipeName { get; set; } = string.Empty;
        public int Count { get; set; }  
        public int PeopleFed { get; set; } 
    }

    public class UnusedIngredientDto
    {
        public string IngredientName { get; set; } = string.Empty;
        public int Remaining { get; set; }
    }

}
