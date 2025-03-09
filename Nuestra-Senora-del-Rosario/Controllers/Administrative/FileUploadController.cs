using Infrastructure.Services.Administrative.GogleDrive;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class FileUploadController : ControllerBase
{
    private readonly ISvGoogleDrive _googleDrive;

    public FileUploadController(ISvGoogleDrive googleDrive)
    {
        _googleDrive = googleDrive;
    }

    /// <summary>
    /// Sube un archivo a la carpeta correspondiente a la cédula indicada.
    /// </summary>
    [HttpPost("upload/{cedula}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadFile(string cedula, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No se proporcionó archivo.");

        try
        {
            // Aseguramos la carpeta con nombre del residente
            var folderId = await _googleDrive.EnsureResidentFolderAsync(cedula);

            // Subir archivo, conservando el nombre original, o si quieres cambia
            var fileName = file.FileName;

            using var stream = file.OpenReadStream();
            var fileId = await _googleDrive.UploadFileAsync(stream, fileName, folderId);

            return Ok(new { message = "Archivo subido con éxito", driveFileId = fileId });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Lista los archivos existentes en la carpeta Drive de un residente (según su cédula).
    /// </summary>
    [HttpGet("list/{cedula}")]
    public async Task<IActionResult> ListFilesByCedula(string cedula)
    {
        try
        {
            // Obtenemos la carpeta (y la creamos si no existiese).
            var folderId = await _googleDrive.EnsureResidentFolderAsync(cedula);

            // Llamamos a un método (que luego definiremos) que liste archivos en esa carpeta.
            var fileList = await _googleDrive.ListFilesInFolderAsync(folderId);

            // Podríamos retornar la lista tal cual o mapearla a un DTO.
            return Ok(fileList);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al listar archivos: {ex.Message}");
        }
    }

    /// <summary>
    /// Elimina un archivo de Drive según su ID.
    /// </summary>
    [HttpDelete("delete/{fileId}")]
    public async Task<IActionResult> DeleteFile(string fileId)
    {
        try
        {
            await _googleDrive.DeleteFileAsync(fileId);
            return Ok(new { message = "Archivo eliminado correctamente." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al eliminar el archivo: {ex.Message}");
        }
    }

    /// <summary>
    /// Descarga un archivo desde Drive por su fileId y lo retorna como FileStreamResult.
    /// </summary>
    [HttpGet("download/{fileId}")]
    public async Task<IActionResult> DownloadFile(string fileId)
    {
        try
        {
            // Recuperamos el stream y el nombre
            var (stream, fileName) = await _googleDrive.DownloadFileAsync(fileId);

            // Regresamos como File. El content-type real lo podrías detectar si lo deseas,
            // aquí se pone "application/octet-stream" de forma genérica.
            return File(stream, "application/octet-stream", fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al descargar el archivo: {ex.Message}");
        }
    }

    /// <summary>
    /// Permite renombrar un archivo por su ID. Se recibe un "newName" en la ruta o en el body.
    /// </summary>
    [HttpPut("rename/{fileId}")]
    public async Task<IActionResult> RenameFile(string fileId, [FromQuery] string newName)
    {
        // newName lo puedes recibir en el body si prefieres: [FromBody] { newName: "X" }
        try
        {
            var updated = await _googleDrive.RenameFileAsync(fileId, newName);
            return Ok(new
            {
                message = "Nombre actualizado con éxito",
                updatedId = updated.Id,
                updatedName = updated.Name
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al renombrar el archivo: {ex.Message}");
        }
    }
}
