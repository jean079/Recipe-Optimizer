using Microsoft.AspNetCore.Mvc;
using RecipeOptimizer.Application.DTO;
using RecipeOptimizer.Application.Interfaces;

namespace RecipeOptimizer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        /// <summary>
        /// Get current inventory
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<InventoryDto>>> GetInventory()
        {
            var inventory = await _inventoryService.GetInventoryAsync();
            return Ok(inventory);
        }
    }
}