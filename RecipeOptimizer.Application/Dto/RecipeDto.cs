using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeOptimizer.Application.DTO
{
    public class RecipeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Feeds { get; set; }
        public List<RecipeIngredientDto> Ingredients { get; set; } = new();
    }
}
