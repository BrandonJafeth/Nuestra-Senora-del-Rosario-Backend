using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class FileUploadController : ControllerBase
{
    private readonly string _storagePath = @"C:\Nuestra Señora del Rosario back\Services\Archivitos\";  // Cambia la ruta según donde quieras guardar

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No se seleccionó ningún archivo.");
        }

        // Generar una ruta completa donde se guardará el archivo
        var filePath = Path.Combine(_storagePath, file.FileName);

        try
        {
            // Guardar el archivo en la ruta especificada
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { message = "Archivo subido correctamente", filePath });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al guardar el archivo: {ex.Message}");
        }
    }
}
