using Microsoft.AspNetCore.JsonPatch;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Administrative.Product
{
    public interface ISvProductService
    {
        Task<IEnumerable<ProductGetDTO>> GetAllProductsAsync();
        Task<ProductGetDTO> GetProductByIdAsync(int productId);
        Task CreateProductAsync(ProductCreateDTO productCreateDTO);

        // Usa el alias ProductEntity para evitar el conflicto
        Task PatchProductAsync(int productId, JsonPatchDocument<Entities.Administration.Product> patchDoc);

        Task DeleteProductAsync(int productId);
    }
}
