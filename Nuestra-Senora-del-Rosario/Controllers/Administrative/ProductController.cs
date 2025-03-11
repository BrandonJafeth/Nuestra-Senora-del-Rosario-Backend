
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.Product;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly ISvProductService _productService;

    public ProductController(ISvProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts(int pageNumber = 1, int pageSize = 10)
    {
        var result = await _productService.GetAllProductsAsync(pageNumber, pageSize);
        return Ok(new
        {
            products = result.Products,
            totalPages = result.TotalPages
        });
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound($"Product with ID {id} not found.");
        }
        return Ok(product);
    }

    [HttpGet("converted/{productId}")]
    public async Task<IActionResult> GetConvertedProduct(int productId, [FromQuery] string targetUnit)
    {
        var productDto = await _productService.GetConvertedProductByIdAsync(productId, targetUnit);
        return Ok(productDto);
    }

    // GET: api/product/bycategory
    // Ejemplo de uso: /api/product/bycategory?categoryId=5&pageNumber=1&pageSize=10
    [HttpGet("bycategory")]
    public async Task<IActionResult> GetProductsByCategory(
        [FromQuery] int categoryId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _productService.GetProductsByCategoryAsync(categoryId, pageNumber, pageSize);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDTO productCreateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _productService.CreateProductAsync(productCreateDto);
        return CreatedAtAction(nameof(GetProductById), new { id = productCreateDto.Name }, productCreateDto);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchProduct(int id, [FromBody] ProductPatchDto patchDto)
    {
        if (patchDto == null)
        {
            return BadRequest("Invalid patch data.");
        }

        try
        {
            await _productService.PatchProductAsync(id, patchDto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }




    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        await _productService.DeleteProductAsync(id);
        return NoContent();
    }
}
