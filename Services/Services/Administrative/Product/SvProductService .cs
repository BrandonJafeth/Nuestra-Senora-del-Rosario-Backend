using AutoMapper;
using Services.GenericService;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Services.Administrative.Product;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Domain.Entities.Administration;
using FluentValidation;
using FluentValidation.Results;

public class SvProductService : ISvProductService
{
    private readonly ISvGenericRepository<Product> _productRepository;
    private readonly ISvGenericRepository<Category> _categoryRepository;
    private readonly ISvGenericRepository<UnitOfMeasure> _unitOfMeasureRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<ProductCreateDTO> _productCreateValidator;
    private readonly IValidator<ProductPatchDto> _productPatchValidator;

    public SvProductService(
        ISvGenericRepository<Product> productRepository,
        ISvGenericRepository<Category> categoryRepository,
        ISvGenericRepository<UnitOfMeasure> unitOfMeasureRepository,
        IMapper mapper, IValidator<ProductCreateDTO> productCreateValidator,
        IValidator<ProductPatchDto> productPatchValidator)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfMeasureRepository = unitOfMeasureRepository;
        _mapper = mapper;
        _productCreateValidator = productCreateValidator;
        _productPatchValidator = productPatchValidator;
    }

    public async Task<(IEnumerable<ProductGetDTO> Products, int TotalPages)> GetAllProductsAsync(int pageNumber, int pageSize)
    {
        var totalProducts = await _productRepository.Query().CountAsync();
        var products = await _productRepository
            .Query()
            .Include(p => p.Category)
            .Include(p => p.UnitOfMeasure)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
        return (_mapper.Map<IEnumerable<ProductGetDTO>>(products), totalPages);
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
        ValidationResult result = await _productCreateValidator.ValidateAsync(productCreateDTO);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);

        var product = _mapper.Map<Product>(productCreateDTO);
        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();
    }

    public async Task PatchProductAsync(int productId, ProductPatchDto patchDto)
    {
        ValidationResult result = await _productPatchValidator.ValidateAsync(patchDto);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);

        var product = await _productRepository
            .Query()
            .Include(p => p.Category)
            .Include(p => p.UnitOfMeasure)
            .FirstOrDefaultAsync(p => p.ProductID == productId);

        if (product == null)
            throw new KeyNotFoundException($"Product with ID {productId} not found");

        if (patchDto.CategoryID.HasValue)
        {
            var category = await _categoryRepository.GetByIdAsync(patchDto.CategoryID.Value);
            if (category == null)
                throw new KeyNotFoundException($"Category with ID {patchDto.CategoryID.Value} not found");

            product.CategoryID = patchDto.CategoryID.Value;
        }

        if (patchDto.UnitOfMeasureID.HasValue)
        {
            var unitOfMeasure = await _unitOfMeasureRepository.GetByIdAsync(patchDto.UnitOfMeasureID.Value);
            if (unitOfMeasure == null)
                throw new KeyNotFoundException($"UnitOfMeasure with ID {patchDto.UnitOfMeasureID.Value} not found");

            product.UnitOfMeasureID = patchDto.UnitOfMeasureID.Value;
        }

        if (patchDto.TotalQuantity.HasValue)
        {
            product.TotalQuantity = patchDto.TotalQuantity.Value;
        }

        if (!string.IsNullOrEmpty(patchDto.Name))
        {
            product.Name = patchDto.Name;
        }

        _productRepository.Update(product);
        await _productRepository.SaveChangesAsync();
    }


    public async Task DeleteProductAsync(int productId)
    {
        await _productRepository.DeleteAsync(productId);
        await _productRepository.SaveChangesAsync();
    }
}
