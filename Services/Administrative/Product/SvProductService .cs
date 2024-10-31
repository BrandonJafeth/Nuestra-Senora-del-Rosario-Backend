using AutoMapper;
using Entities.Administration;
using Services.GenericService;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Services.Administrative.Product;
using Microsoft.EntityFrameworkCore;


public class SvProductService : ISvProductService
{
    private readonly ISvGenericRepository<Product> _productRepository;
    private readonly IMapper _mapper;

    public SvProductService(ISvGenericRepository<Product> productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductGetDTO>> GetAllProductsAsync()
    {
        var products = await _productRepository
            .Query()
            .Include(p => p.Category)        // Incluir Category
            .Include(p => p.UnitOfMeasure)   // Incluir UnitOfMeasure
            .ToListAsync();

        return _mapper.Map<IEnumerable<ProductGetDTO>>(products);
    }

    public async Task<ProductGetDTO> GetProductByIdAsync(int productId)
    {
        var product = await _productRepository
            .Query()
            .Include(p => p.Category)
            .Include(p => p.UnitOfMeasure)
            .FirstOrDefaultAsync(p => p.ProductID == productId);

        if (product == null) throw new KeyNotFoundException($"Product with ID {productId} not found.");
        return _mapper.Map<ProductGetDTO>(product);
    }


    public async Task CreateProductAsync(ProductCreateDTO productCreateDTO)
    {
        var product = _mapper.Map<Product>(productCreateDTO);
        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();
    }

    public async Task PatchProductAsync(int productId, JsonPatchDocument<Product> patchDoc)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null) throw new KeyNotFoundException($"Product with ID {productId} not found.");

        patchDoc.ApplyTo(product);
        await _productRepository.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(int productId)
    {
        await _productRepository.DeleteAsync(productId);
        await _productRepository.SaveChangesAsync();
    }
}
