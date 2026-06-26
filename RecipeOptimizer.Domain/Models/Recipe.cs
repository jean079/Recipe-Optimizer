using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeOptimizer.Domain.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Feeds { get; set; }

        public ICollection<RecipeIngredient> RecipeIngredients { get; set; }
            = new List<RecipeIngredient>();
    }
}
