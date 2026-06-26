using RecipeOptimizer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RecipeOptimizer.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // These DbSets represent your database tables
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<Inventory> Inventories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the Ingredient entity
            modelBuilder.Entity<Ingredient>()
                .HasKey(i => i.Id);

            modelBuilder.Entity<Ingredient>()
                .Property(i => i.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Configure the Recipe entity
            modelBuilder.Entity<Recipe>()
                .HasKey(r => r.Id);

            modelBuilder.Entity<Recipe>()
                .Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Configure the RecipeIngredient entity (junction table)
            modelBuilder.Entity<RecipeIngredient>()
                .HasKey(ri => ri.Id);

            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Recipe)
                .WithMany(r => r.RecipeIngredients)
                .HasForeignKey(ri => ri.RecipeId);

            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Ingredient)
                .WithMany()
                .HasForeignKey(ri => ri.IngredientId);

            // Configure the Inventory entity
            modelBuilder.Entity<Inventory>()
                .HasKey(i => i.Id);

            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.Ingredient)
                .WithMany()
                .HasForeignKey(i => i.IngredientId);

            // Seed initial data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Ingredients (from your image)
            modelBuilder.Entity<Ingredient>().HasData(
                new Ingredient { Id = 1, Name = "Meat" },
                new Ingredient { Id = 2, Name = "Lettuce" },
                new Ingredient { Id = 3, Name = "Tomato" },
                new Ingredient { Id = 4, Name = "Cheese" },
                new Ingredient { Id = 5, Name = "Dough" },
                new Ingredient { Id = 6, Name = "Cucumber" },
                new Ingredient { Id = 7, Name = "Olives" }
            );

            // Seed Recipes (from your image)
            modelBuilder.Entity<Recipe>().HasData(
                new Recipe { Id = 1, Name = "Burger", Feeds = 1 },
                new Recipe { Id = 2, Name = "Pie", Feeds = 1 },
                new Recipe { Id = 3, Name = "Sandwich", Feeds = 1 },
                new Recipe { Id = 4, Name = "Pasta", Feeds = 2 },
                new Recipe { Id = 5, Name = "Salad", Feeds = 3 },
                new Recipe { Id = 6, Name = "Pizza", Feeds = 4 }
            );

            // Seed RecipeIngredients (what each recipe needs)
            modelBuilder.Entity<RecipeIngredient>().HasData(
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
            );

            // Seed Inventory (available ingredients from your image)
            modelBuilder.Entity<Inventory>().HasData(
                new Inventory { Id = 1, IngredientId = 6, AvailableQuantity = 2 },   // 2 x Cucumber
                new Inventory { Id = 2, IngredientId = 7, AvailableQuantity = 2 },   // 2 x Olives
                new Inventory { Id = 3, IngredientId = 2, AvailableQuantity = 3 },   // 3 x Lettuce
                new Inventory { Id = 4, IngredientId = 1, AvailableQuantity = 6 },   // 6 x Meat
                new Inventory { Id = 5, IngredientId = 3, AvailableQuantity = 6 },   // 6 x Tomato
                new Inventory { Id = 6, IngredientId = 4, AvailableQuantity = 8 },   // 8 x Cheese
                new Inventory { Id = 7, IngredientId = 5, AvailableQuantity = 10 }   // 10 x Dough
            );
        }
    }

}
