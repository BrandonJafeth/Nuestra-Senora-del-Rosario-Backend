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
using Infrastructure.Services.Administrative.ConversionService;

public class SvProductService : ISvProductService
{
    private readonly ISvGenericRepository<Product> _productRepository;
    private readonly ISvGenericRepository<Category> _categoryRepository;
    private readonly ISvGenericRepository<UnitOfMeasure> _unitOfMeasureRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<ProductCreateDTO> _productCreateValidator;
    private readonly IValidator<ProductPatchDto> _productPatchValidator;
    private readonly ISvConversionService _conversionService;

    public SvProductService(
        ISvGenericRepository<Product> productRepository,
        ISvGenericRepository<Category> categoryRepository,
        ISvGenericRepository<UnitOfMeasure> unitOfMeasureRepository,
        IMapper mapper, IValidator<ProductCreateDTO> productCreateValidator,
        IValidator<ProductPatchDto> productPatchValidator,
          ISvConversionService conversionService)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfMeasureRepository = unitOfMeasureRepository;
        _mapper = mapper;
        _productCreateValidator = productCreateValidator;
        _productPatchValidator = productPatchValidator;
        _conversionService = conversionService;
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

    public async Task<(IEnumerable<ProductGetDTO> Products, int TotalPages)> GetProductsByCategoryAsync(int categoryId, int pageNumber, int pageSize)
    {
        // Consulta de productos filtrados por categoría
        var query = _productRepository.Query()
            .Include(p => p.Category)
            .Include(p => p.UnitOfMeasure)
            .Where(p => p.CategoryID == categoryId);

        int totalProducts = await query.CountAsync();
        var products = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        int totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

        var result = _mapper.Map<IEnumerable<ProductGetDTO>>(products);
        return (result, totalPages);
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


    public async Task<ProductGetConvertDTO> GetConvertedProductByIdAsync(int productId, string targetUnit)
    {
        // 1. Obtener el producto con sus relaciones
        var product = await _productRepository.Query()
            .Include(p => p.Category)
            .Include(p => p.UnitOfMeasure)
            .FirstOrDefaultAsync(p => p.ProductID == productId);

        if (product == null)
            throw new KeyNotFoundException($"No se encontró el producto con ID {productId}.");

        // 2. Mapear a DTO
        var dto = _mapper.Map<ProductGetConvertDTO>(product);

        // 3. Inicializar los campos de conversión
        dto.ConvertedTotalQuantity = dto.TotalQuantity;
        dto.ConvertedUnitOfMeasure = dto.UnitOfMeasure;

        // 4. Aplicar conversión si se indica un targetUnit
        if (!string.IsNullOrEmpty(targetUnit))
        {
            double converted = dto.TotalQuantity;
            string measure = dto.UnitOfMeasure ?? "Unknown";

            switch (targetUnit.ToLower())
            {
                case "paquete":
                    // Ejemplo: 1 paquete = 20 unidades
                    converted = dto.TotalQuantity / 20.0;
                    measure = "Paquete(s)";
                    break;
                case "kg":
                    // Ejemplo: si el producto está en gramos, 1 kg = 1000 gramos
                    converted = _conversionService.ConvertGramsToKilograms(dto.TotalQuantity);
                    measure = "kg";
                    break;
                case "caja":
                    // Ejemplo: si el producto se mide en litros y 1 caja = 12 litros
                    if (dto.UnitOfMeasure.ToLower().Contains("litro"))
                    {
                        var milkResult = _conversionService.ConvertMilk(dto.TotalQuantity, 12);
                        converted = milkResult.Boxes;
                        measure = "Caja(s)";
                    }
                    break;
                // Agrega más casos según tu lógica
                default:
                    // No se hace conversión
                    break;
            }

            dto.ConvertedTotalQuantity = converted;
            dto.ConvertedUnitOfMeasure = measure;
        }

        return dto;
    }

}
