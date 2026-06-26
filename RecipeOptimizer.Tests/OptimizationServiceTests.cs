using Xunit;
using RecipeOptimizer.Application.Services;
using RecipeOptimizer.Domain.Models;
using RecipeOptimizer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace RecipeOptimizer.Tests
{
    public class OptimizationServiceTests
    {
        private AppDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);
            SeedTestData(context);
            return context;
        }

        private void SeedTestData(AppDbContext context)
        {
            // Add test ingredients
            var ingredients = new[]
            {
                new Ingredient { Id = 1, Name = "Meat" },
                new Ingredient { Id = 2, Name = "Lettuce" },
                new Ingredient { Id = 3, Name = "Tomato" },
                new Ingredient { Id = 4, Name = "Cheese" },
                new Ingredient { Id = 5, Name = "Dough" },
                new Ingredient { Id = 6, Name = "Cucumber" },
                new Ingredient { Id = 7, Name = "Olives" }
            };
            context.Ingredients.AddRange(ingredients);

            // Add test recipes
            var recipes = new[]
            {
                new Recipe { Id = 1, Name = "Burger", Feeds = 1 },
                new Recipe { Id = 2, Name = "Pie", Feeds = 1 },
                new Recipe { Id = 3, Name = "Sandwich", Feeds = 1 } ,
                new Recipe { Id = 4, Name = "Pasta", Feeds = 2 },
                new Recipe { Id = 5, Name = "Salad", Feeds = 3 } ,
                new Recipe { Id = 6, Name = "Pizza", Feeds = 4 }
            };
            context.Recipes.AddRange(recipes);

            // Add recipe ingredients
            var recipeIngredients = new[]
            {
                // Burger: 1 Meat, 1 Lettuce, 1 Tomato, 1 Cheese, 1 Dough
                new RecipeIngredient { Id = 1, RecipeId = 1, IngredientId = 1, Quantity = 1 },
                new RecipeIngredient { Id = 2, RecipeId = 1, IngredientId = 2, Quantity = 1 },
                new RecipeIngredient { Id = 3, RecipeId = 1, IngredientId = 3, Quantity = 1 },
                new RecipeIngredient { Id = 4, RecipeId = 1, IngredientId = 4, Quantity = 1 },
                new RecipeIngredient { Id = 5, RecipeId = 1, IngredientId = 5, Quantity = 1 },

                // Pie: 2 Dough, 2 Meat
                new RecipeIngredient { Id = 6, RecipeId = 2, IngredientId = 5, Quantity = 2 },
                new RecipeIngredient { Id = 7, RecipeId = 2, IngredientId = 1, Quantity = 2 },

                // Sandwich: 1 Dough, 1 Cucumber
                new RecipeIngredient { Id = 8, RecipeId = 3, IngredientId = 5, Quantity = 1 },
                new RecipeIngredient { Id = 9, RecipeId = 3, IngredientId = 6, Quantity = 1 },

                // Pasta: 2 Dough, 1 Tomato, 2 Cheese, 1 Meat
                new RecipeIngredient { Id = 10, RecipeId = 4, IngredientId = 5, Quantity = 2 },
                new RecipeIngredient { Id = 11, RecipeId = 4, IngredientId = 3, Quantity = 1 },
                new RecipeIngredient { Id = 12, RecipeId = 4, IngredientId = 4, Quantity = 2 },
                new RecipeIngredient { Id = 13, RecipeId = 4, IngredientId = 1, Quantity = 1 },

                // Salad: 2 Lettuce, 2 Tomato, 1 Cucumber, 2 Cheese, 1 Olives
                new RecipeIngredient { Id = 14, RecipeId = 5, IngredientId = 2, Quantity = 2 },
                new RecipeIngredient { Id = 15, RecipeId = 5, IngredientId = 3, Quantity = 2 },
                new RecipeIngredient { Id = 16, RecipeId = 5, IngredientId = 6, Quantity = 1 },
                new RecipeIngredient { Id = 17, RecipeId = 5, IngredientId = 4, Quantity = 2 },
                new RecipeIngredient { Id = 18, RecipeId = 5, IngredientId = 7, Quantity = 1 },

                // Pizza: 3 Dough, 2 Tomato, 3 Cheese, 1 Olives
                new RecipeIngredient { Id = 19, RecipeId = 6, IngredientId = 5, Quantity = 3 },
                new RecipeIngredient { Id = 20, RecipeId = 6, IngredientId = 3, Quantity = 2 },
                new RecipeIngredient { Id = 21, RecipeId = 6, IngredientId = 4, Quantity = 3 },
                new RecipeIngredient { Id = 22, RecipeId = 6, IngredientId = 7, Quantity = 1 }
            };
            context.RecipeIngredients.AddRange(recipeIngredients);

            context.SaveChanges();
        }

        [Fact]
        public void FindOptimalCombination_WithEmptyInventory_ReturnsZeroPeopleFed()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new OptimizationService(context);
            var recipes = context.Recipes.Include(r => r.RecipeIngredients).ToList();
            var emptyInventory = new Dictionary<int, int>();

            // Act
            var result = service.FindOptimalCombination(recipes, emptyInventory);

            // Assert
            Assert.Equal(0, result.TotalPeopleFed);
            Assert.Empty(result.MealPlan);
        }

        [Fact]
        public void FindOptimalCombination_CanMakeSingleRecipe_ReturnsSingleMeal()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new OptimizationService(context);
            var recipes = context.Recipes.Include(r => r.RecipeIngredients).ToList();

            // Inventory for one Burger: 1 Meat, 1 Lettuce, 1 Tomato, 1 Cheese, 1 Dough
            var inventory = new Dictionary<int, int>
            {
                { 1, 1 },  // Meat
                { 2, 1 },  // Lettuce
                { 3, 1 },  // Tomato
                { 4, 1 },  // Cheese
                { 5, 1 }   // Dough
            };

            // Act
            var result = service.FindOptimalCombination(recipes, inventory);

            //Assert
            Assert.Equal(1, result.TotalPeopleFed);
            Assert.Single(result.MealPlan);
            Assert.Equal("Burger", result.MealPlan[0].RecipeName);
            Assert.Equal(1, result.MealPlan[0].Count);
        }

        [Fact]
        public void FindOptimalCombination_CanMakeMultipleRecipes_ReturnsOptimalCombination()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new OptimizationService(context);
            var recipes = context.Recipes.Include(r => r.RecipeIngredients).ToList();

            // Inventory matching the problem statement
            var inventory = new Dictionary<int, int>
            {
                { 1, 6 },   //6 x Meat
                { 2, 3 },   //3 x Lettuce
                { 3, 6 },   //6 x Tomato
                { 4, 8 },   //8 x Cheese
                { 5, 10 },  //10 x Dough
                { 6, 2 },   //2 x Cucumber
                { 7, 2 }    //2 x Olives
            };

            // Act
            var result = service.FindOptimalCombination(recipes, inventory);

            Assert.NotNull(result);
            Assert.True(result.TotalPeopleFed > 0, "Should be able to feed someone with this inventory");
            Assert.NotEmpty(result.MealPlan);

            // Verify it's a valid combination (sum of people fed)
            int totalPeopleFed = result.MealPlan.Sum(m => m.PeopleFed);
            Assert.Equal(result.TotalPeopleFed, totalPeopleFed);
        }

        [Fact]
        public void FindOptimalCombination_WithLimitedIngredients_SkipsMissingRecipes()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new OptimizationService(context);
            var recipes = context.Recipes.Include(r => r.RecipeIngredients).ToList();

            // Only have ingredients for Sandwich (1 Dough, 1 Cucumber)
            var inventory = new Dictionary<int, int>
            {
                { 5, 5 },   // 5 x Dough
                { 6, 5 }    // 5 x Cucumber
            };

            // Act
            var result = service.FindOptimalCombination(recipes, inventory);

            // Assert
            // Should be able to make 5 sandwiches (1 feeds 1 person each = 5 people)
            Assert.Equal(5, result.TotalPeopleFed);
            Assert.Single(result.MealPlan);
            Assert.Equal("Sandwich", result.MealPlan[0].RecipeName);
            Assert.Equal(5, result.MealPlan[0].Count);
        }

        [Fact]
        public void FindOptimalCombination_TracksUnusedIngredients()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new OptimizationService(context);
            var recipes = context.Recipes.Include(r => r.RecipeIngredients).ToList();

            // Ingredients for one sandwich with extra Dough
            var inventory = new Dictionary<int, int>
            {
                { 5, 10 },  // 10 x Dough (only need 1)
                { 6, 1 }    // 1 x Cucumber
            };

            // Act
            var result = service.FindOptimalCombination(recipes, inventory);

            // Assert
            Assert.NotEmpty(result.UnusedIngredients);
            var unusedDough = result.UnusedIngredients.FirstOrDefault(u => u.IngredientName == "Dough");
            Assert.NotNull(unusedDough);
            Assert.Equal(9, unusedDough.Remaining);  // 10 - 1 = 9
        }

        [Fact]
        public void FindOptimalCombination_PreferRecipesThatFeedMore_WhenOptimal()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new OptimizationService(context);
            var recipes = context.Recipes.Include(r => r.RecipeIngredients).ToList();

            // Enough for either 1 Pizza (feeds 4) or 2 Salads (feeds 6)
            var inventory = new Dictionary<int, int>
            {
                { 2, 4 },   // 4 x Lettuce
                { 3, 4 },   //  4 x Tomato
                { 4, 7 },   // 7 x Cheese (Pizza needs 3, Salad needs 2)
                { 5, 3 },   // 3 x Dough (Pizza needs 3)
                { 6, 2 },   // 2 x Cucumber
                { 7, 2 }    // 2 x Olives
            };

            // Act
            var result = service.FindOptimalCombination(recipes, inventory);

            // Assert
            Assert.True(result.TotalPeopleFed >= 4, "Should feed at least 4 people (Pizza) or more (Salad)");
        }
    }
}