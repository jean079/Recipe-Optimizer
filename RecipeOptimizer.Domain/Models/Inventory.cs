using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeOptimizer.Domain.Models
{
    public class Inventory
    {
        public int Id { get; set; }
        public int IngredientId { get; set; }
        public int AvailableQuantity { get; set; }

        public Ingredient? Ingredient { get; set; }
    }

}
