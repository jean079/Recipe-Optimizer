# Recipe Optimizer
 
A C# .NET backend application that finds the best combination of recipes to feed the maximum number of people using available ingredients.
 
## What Does It Do?
 
Given a list of available ingredients and recipes, this application determines which recipes you should make to feed as many people as possible. 
 
**Example:**
- You have: 6 Meat, 10 Dough, 8 Cheese, and other ingredients
- Available recipes: Burger, Pizza, Pasta, Salad, Sandwich, Pie
- The application finds: Make 2 Pizza and 3 Burgers to feed 11 people
---
 
## How to Get Started
 
### 1. Requirements
 
You need to have installed:
- **.NET 9.0 SDK** - Download from https://dotnet.microsoft.com/download
- A code editor (Visual Studio 2022, VS Code, or any text editor)
### 2. Clone or Download the Project
 
```bash
cd RecipeOptimizer
```
 
### 3. Run the Application
 
First, create the database:
 
```bash
cd RecipeOptimizer.Api
dotnet run
```
 
The application will:
- Create a SQLite database automatically
- Fill it with test data (recipes and ingredients)
- Start on: `https://localhost:5211`
### 4. View the API
 
Open your browser and go to:
 
```
https://localhost:5211/swagger/index.html
```
 
You will see a page with all available API commands. Click "Try it out" to test each one.
 
---
 
## API Endpoints (What You Can Do)
 
### 1. Get All Recipes
```
GET /api/recipes
```
Returns a list of all available recipes.
 
**Example Response:**
```json
[
  {
    "id": 1,
    "name": "Burger",
    "feeds": 1,
    "ingredients": [
      { "ingredientId": 1, "ingredientName": "Meat", "quantity": 1 }
    ]
  }
]
```
 
---
 
### 2. Get One Recipe
```
GET /api/recipes/{id}
```
Returns details about a specific recipe by ID.
 
---
 
### 3. Get Current Ingredients
```
GET /api/inventory
```
Shows how much of each ingredient you have available.
 
**Example Response:**
```json
[
  { "ingredientId": 1, "ingredientName": "Meat", "availableQuantity": 6 },
  { "ingredientId": 5, "ingredientName": "Dough", "availableQuantity": 10 }
]
```
 
---
 
### 4. Get Recipes You Can Make Now
```
GET /api/optimization/possible-recipes
```
Shows which recipes you can actually make with your current ingredients.
 
---
 
### 5. Get the Best Recipe Combination (The Main Feature)
```
GET /api/optimization/optimal-combination
```
This is the core feature. It finds the best combination of recipes to feed the most people.
 
**Example Response:**
```json
{
  "totalPeopleFed": 11,
  "mealPlan": [
    {
      "recipeId": 6,
      "recipeName": "Pizza",
      "count": 2,
      "peopleFed": 8
    },
    {
      "recipeId": 1,
      "recipeName": "Burger",
      "count": 3,
      "peopleFed": 3
    }
  ],
  "unusedIngredients": [
    { "ingredientName": "Cucumber", "remaining": 2 },
    { "ingredientName": "Olives", "remaining": 1 }
  ]
}
```
 
---
 
## How It Works (The Architecture)
 
The project is organized into 4 layers:
 
### 1. **Domain Layer** (`RecipeOptimizer.Domain`)
This contains the basic business objects:
- `Ingredient` - What goes into recipes (Meat, Cheese, etc.)
- `Recipe` - Instructions for making food
- `RecipeIngredient` - Links recipes to ingredients
- `Inventory` - How much of each ingredient we have
**Think of it as:** The rules of the game
 
---
 
### 2. **Infrastructure Layer** (`RecipeOptimizer.Infrastructure`)
This handles the database:
- `AppDbContext` - Connection to SQLite database
- `Repository` - Gets data from the database
- Database seeding - Fills the database with test data
**Think of it as:** The storage room
 
---
 
### 3. **Application Layer** (`RecipeOptimizer.Application`)
This contains the business logic:
- `RecipeService` - Fetches recipes from database
- `InventoryService` - Fetches available ingredients
- `OptimizationService` - **The brain** - finds the best recipe combination
**Think of it as:** The thinking part
 
---
 
### 4. **API Layer** (`RecipeOptimizer.Api`)
This exposes the services as REST endpoints:
- `RecipesController` - Endpoints for recipes
- `InventoryController` - Endpoints for inventory
- `OptimizationController` - Endpoints for finding best combination
**Think of it as:** The front desk that talks to users
 
---
 
## The Algorithm (How It Finds the Best Combination)
 
The optimization service uses **Backtracking** - a smart method that:
 
1. Tries making one recipe
2. With remaining ingredients, tries making another recipe
3. Keeps track of the best solution found
4. Goes back and tries different combinations
**Why this approach?**
- It guarantees finding the actual best answer (not just a good guess)
- It explores all possibilities efficiently
- It's easy to understand and improve later
**How fast is it?**
- With the test data (6 recipes, 7 ingredients): **Instant**
- With 20 recipes and 50 ingredients: **Still fast**
- If you add 100+ recipes: You might want to use caching or dynamic programming
---
 
## Running Tests
 
Tests check if the algorithm works correctly:
 
```bash
cd RecipeOptimizer.Tests
dotnet test
```
 
**What we test:**
- Empty inventory (can we handle having nothing?)
- Making one recipe
- Making multiple recipes
- Limited ingredients (can we handle missing items?)
- Unused ingredients are tracked correctly
- Recipes that feed more people are preferred
All tests should **Pass** ✅
 
---
 
## Project Structure
 
```
RecipeOptimizer/
├── RecipeOptimizer.Domain/           (Business objects)
│   └── Models/
│       ├── Ingredient.cs
│       ├── Recipe.cs
│       ├── RecipeIngredient.cs
│       └── Inventory.cs
│
├── RecipeOptimizer.Infrastructure/   (Database)
│   ├── Data/
│   │   └── AppDbContext.cs
│   └── Repositories/
│       ├── IRepository.cs
│       └── Repository.cs
│
├── RecipeOptimizer.Application/      (Business logic)
│   ├── DTOs/                         (Data for API responses)
│   │   ├── IngredientDto.cs
│   │   ├── RecipeDto.cs
│   │   ├── MealCombinationDto.cs
│   │   └── ...
│   ├── Interfaces/                   (Contracts)
│   │   ├── IRecipeService.cs
│   │   ├── IInventoryService.cs
│   │   └── IOptimizationService.cs
│   └── Services/                     (The actual logic)
│       ├── RecipeService.cs
│       ├── InventoryService.cs
│       └── OptimizationService.cs
│
├── RecipeOptimizer.Api/              (REST API)
│   ├── Controllers/
│   │   ├── RecipesController.cs
│   │   ├── InventoryController.cs
│   │   └── OptimizationController.cs
│   ├── Program.cs                    (Startup configuration)
│   └── Properties/
│       └── launchSettings.json       (Where to run)
│
├── RecipeOptimizer.Tests/            (Unit tests)
│   └── OptimizationServiceTests.cs
│
└── RecipeOptimizer.sln               (Solution file)
```
 
---
 
## Design Choices and Why
 
### 1. **Clean Architecture**
We separated the code into 4 layers so that:
- Each layer has one job
- Easy to test each part separately
- Easy to change one layer without breaking others
- Easy for new developers to understand
---
 
### 2. **Dependency Injection**
Services receive their dependencies (like database context) instead of creating them. Why?
- Makes testing easier (we can use fake databases)
- Makes code more flexible
- Follows industry best practices
---
 
### 3. **Repository Pattern**
We created a repository layer between the service and database. Why?
- If you want to switch from SQLite to SQL Server later, you only change one place
- Makes it easier to test services with fake data
- Keeps business logic separate from database logic
---
 
### 4. **DTOs (Data Transfer Objects)**
We don't send database models directly to the API. We create DTOs instead. Why?
- Hides internal structure from the outside world
- You can change the database without breaking the API
- You can include extra calculated fields (like unused ingredients)
---
 
### 5. **SQLite Database**
We use SQLite instead of SQL Server. Why?
- No setup needed - it's just a file
- You can clone the project and run it immediately
- Good enough for testing and small projects
- Easy to switch to SQL Server later if needed
---
 
## How to Extend This Project
 
### Add a New Recipe
1. Open the database (it's in `RecipeOptimizer.Api/`)
2. Add ingredients to the Ingredients table
3. Add the recipe to the Recipes table
4. Link them in RecipeIngredients table
Or modify `SeedData()` in `AppDbContext.cs`
 
---
 
### Improve Performance
If you add many more recipes:
- Add **memoization** to remember previous calculations
- Add **caching** for recipe lookups
- Consider **dynamic programming** for large datasets
---
 
### Add a Frontend
The API is ready for any frontend (Angular, React, Vue, etc.):
- All endpoints return JSON
- CORS (cross-origin requests) is already enabled
- Swagger documentation is available
---
 
## Database
 
The database is SQLite and contains 4 tables:
 
**Ingredients** - Available items
- Id, Name
**Recipes** - Instructions for meals
- Id, Name, Feeds (how many people it feeds)
**RecipeIngredients** - Links recipes to ingredients
- Id, RecipeId, IngredientId, Quantity
**Inventory** - How much of each ingredient we have
- Id, IngredientId, AvailableQuantity
---
 
## Troubleshooting
 
**Problem:** "Could not find project to run"
```
cd RecipeOptimizer.Api
dotnet run
```
 
**Problem:** Database errors
```
dotnet clean
dotnet build
dotnet run
```
 
**Problem:** Port already in use
Check `launchSettings.json` and change the port number.
 
---
 
## Questions or Issues?
 
If something doesn't work:
1. Make sure .NET 9.0 is installed: `dotnet --version`
2. Try cleaning and rebuilding: `dotnet clean && dotnet build`
3. Delete the database file and restart: `recipeoptimizer.db`
---
 
## Summary
 
This project shows:
- ✅ Clean Architecture (4 layers)
- ✅ Database integration (SQLite)
- ✅ REST APIs (Swagger included)
- ✅ Smart algorithm (Backtracking)
- ✅ Unit tests (6 comprehensive tests)
- ✅ Professional code structure
- ✅ Ready to scale
Good luck! 🚀
