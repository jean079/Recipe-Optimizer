using RecipeOptimizer.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeOptimizer.Application.Interfaces
{
    public interface IInventoryService
    {
        Task<List<InventoryDto>> GetInventoryAsync();
    }
}
