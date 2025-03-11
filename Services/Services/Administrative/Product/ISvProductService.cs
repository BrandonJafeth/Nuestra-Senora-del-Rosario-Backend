using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.Product
{
    public interface ISvProductService
    {
        Task<(IEnumerable<ProductGetDTO> Products, int TotalPages)> GetAllProductsAsync(int pageNumber, int pageSize);
        Task<ProductGetDTO> GetProductByIdAsync(int productId);

        Task<ProductGetConvertDTO> GetConvertedProductByIdAsync(int productId, string targetUnit);

        Task<(IEnumerable<ProductGetDTO> Products, int TotalPages)> GetProductsByCategoryAsync(int categoryId, int pageNumber, int pageSize);
    

        Task CreateProductAsync(ProductCreateDTO productCreateDTO);

        // Usa el alias ProductEntity para evitar el conflicto
        Task PatchProductAsync(int productId, ProductPatchDto patchDto);

        Task DeleteProductAsync(int productId);
    }
}
