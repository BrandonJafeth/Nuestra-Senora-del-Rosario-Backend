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
        Task<(IEnumerable<ProductGetDTO> Products, int TotalPages)> GetAllProductsAsync(int pageNumber, int pageSize);
        Task<ProductGetDTO> GetProductByIdAsync(int productId);
        Task CreateProductAsync(ProductCreateDTO productCreateDTO);

        // Usa el alias ProductEntity para evitar el conflicto
        Task PatchProductAsync(int productId, ProductPatchDto patchDto);

        Task DeleteProductAsync(int productId);
    }
}
