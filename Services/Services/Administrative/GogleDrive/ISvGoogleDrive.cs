using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.GogleDrive
{
    public interface ISvGoogleDrive
    {
        Task<string> EnsureCedulaFolderAsync(string cedula);
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string folderId);

        // Listar archivos
        Task<IList<Google.Apis.Drive.v3.Data.File>> ListFilesInFolderAsync(string folderId);

        // Eliminar
        Task DeleteFileAsync(string fileId);

        // Descargar
        Task<(MemoryStream Stream, string FileName)> DownloadFileAsync(string fileId);

        // Renombrar
        Task<Google.Apis.Drive.v3.Data.File> RenameFileAsync(string fileId, string newName);

    }
}
