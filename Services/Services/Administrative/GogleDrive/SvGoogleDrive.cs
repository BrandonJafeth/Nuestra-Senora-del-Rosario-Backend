using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Infrastructure.Services.Administrative.Residents;

namespace Infrastructure.Services.Administrative.GogleDrive
{
    public class SvGoogleDrive : ISvGoogleDrive
    {
        private readonly DriveService _driveService;
        private readonly ISvResident _residentService;

        /// <summary>
        /// ID de la carpeta padre donde se van a crear las subcarpetas por cédula.
        /// Ejemplo: 1Yz97xBoW_UIf3QX8FH3zpbTEpPraFU_d
        /// </summary>
        private const string PARENT_FOLDER_ID = "1Yz97xBoW_UIf3QX8FH3zpbTEpPraFU_d";

        public SvGoogleDrive(ISvResident residentService)
        {

            _residentService = residentService;
            // 1. Leer credenciales desde variable de entorno
            var credentialsJson = Environment.GetEnvironmentVariable("GOOGLE_CREDENTIALS_JSON");
            if (string.IsNullOrEmpty(credentialsJson))
            {
                throw new Exception("No se encontró la variable de entorno GOOGLE_CREDENTIALS_JSON con las credenciales de Google.");
            }

            // 2. Crear GoogleCredential en memoria
            var credential = GoogleCredential.FromJson(credentialsJson)
                .CreateScoped(DriveService.Scope.Drive, DriveService.Scope.DriveFile);

            // 3. Inicializar el DriveService
            _driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "MiAppHogarAncianos"
            });
        }

        /// <summary>
        /// Verifica si existe la carpeta con el nombre (cédula) dentro de la carpeta padre.
        /// Si no existe, la crea y le asigna permisos de lectura pública.
        /// Retorna el Id de la carpeta final.
        /// </summary>
        /// <param name="cedula">Nombre de la subcarpeta (usualmente la cédula del residente).</param>
        /// <returns>Id de la carpeta en Google Drive.</returns>
        public async Task<string> EnsureCedulaFolderAsync(string cedula)
        {
            // 1. Buscar si ya existe la carpeta con ese nombre en la carpeta padre.
            var listRequest = _driveService.Files.List();
            listRequest.Q = $"name = '{cedula}' " +
                            "and mimeType = 'application/vnd.google-apps.folder' " +
                            "and trashed = false " +
                            $"and '{PARENT_FOLDER_ID}' in parents";
            listRequest.Fields = "files(id, name)";

            var result = await listRequest.ExecuteAsync();
            if (result.Files?.Count > 0)
            {
                // Ya existe; devolvemos su Id.
                return result.Files[0].Id;
            }

            // 2. Crear la carpeta dentro de la carpeta padre.
            var folderMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = cedula,
                MimeType = "application/vnd.google-apps.folder",
                Parents = new List<string> { PARENT_FOLDER_ID }
            };

            var createRequest = _driveService.Files.Create(folderMetadata);
            createRequest.Fields = "id";
            var folder = await createRequest.ExecuteAsync();

            // 3. Otorgar permisos de lectura pública ("anyone" -> "reader")
            await GrantPublicReadAccessAsync(folder.Id);

            return folder.Id;
        }

        /// <summary>
        /// Sube un archivo a la carpeta indicada (folderId) y, si se requiere, 
        /// también se le asigna permiso de lectura pública.
        /// </summary>
        /// <param name="fileStream">Stream del archivo que se quiere subir.</param>
        /// <param name="fileName">Nombre con el que se guardará en Drive.</param>
        /// <param name="folderId">Id de la carpeta destino en Google Drive.</param>
        /// <returns>Id del archivo subido.</returns>
        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string folderId)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = fileName,
                Parents = new List<string> { folderId }
            };

            var request = _driveService.Files.Create(
                fileMetadata,
                fileStream,
                "application/octet-stream"
            );

            request.Fields = "id, name, parents";

            var uploadResult = await request.UploadAsync();
            if (uploadResult.Status != UploadStatus.Completed)
            {
                throw new Exception($"No se pudo subir el archivo: {uploadResult.Exception?.Message}");
            }

            // Si quieres que cada archivo también sea accesible sin pedir acceso,
            // descomenta la siguiente línea:
            // await GrantPublicReadAccessAsync(request.ResponseBody.Id);

            return request.ResponseBody.Id;
        }

        /// <summary>
        /// Otorga permisos de lectura pública ("anyone" -> "reader") 
        /// al recurso de Drive (archivo o carpeta) especificado.
        /// </summary>
        /// <param name="driveItemId">Id del archivo o carpeta en Drive.</param>
        public async Task GrantPublicReadAccessAsync(string driveItemId)
        {
            var permission = new Permission
            {
                Type = "anyone",
                Role = "reader"
            };

            var request = _driveService.Permissions.Create(permission, driveItemId);
            request.Fields = "id";

            await request.ExecuteAsync();
        }

        // ------------------ NUEVOS MÉTODOS ------------------

        /// <summary>
        /// Lista los archivos de una carpeta dada.
        /// </summary>
        public async Task<IList<Google.Apis.Drive.v3.Data.File>> ListFilesInFolderAsync(string folderId)
        {
            var listRequest = _driveService.Files.List();
            listRequest.Q = $"'{folderId}' in parents and trashed = false";
            // Puedes añadir: and mimeType != 'application/vnd.google-apps.folder' 
            // si quieres excluir subcarpetas, etc.

            // Pide los campos que requieras
            listRequest.Fields = "files(id, name, webViewLink, webContentLink, mimeType, size)";

            var result = await listRequest.ExecuteAsync();
            return result.Files;
        }

        /// <summary>
        /// Elimina un archivo en Drive por su fileId.
        /// </summary>
        public async Task DeleteFileAsync(string fileId)
        {
            var deleteRequest = _driveService.Files.Delete(fileId);
            await deleteRequest.ExecuteAsync();
        }

        /// <summary>
        /// Descarga un archivo y devuelve (stream, fileName).
        /// </summary>
        public async Task<(MemoryStream Stream, string FileName)> DownloadFileAsync(string fileId)
        {
            // 1. Obtener el file metadata para saber el nombre
            var getRequest = _driveService.Files.Get(fileId);
            getRequest.Fields = "id, name, mimeType";  // o lo que requieras
            var file = await getRequest.ExecuteAsync();

            // 2. Descargar el contenido en memoria
            var stream = new MemoryStream();
            await getRequest.DownloadAsync(stream);

            // Regresar el stream y el nombre
            // Importante: poner la posición del stream al inicio:
            stream.Position = 0;

            return (stream, file.Name);
        }

        /// <summary>
        /// Renombra un archivo en Drive por su fileId.
        /// </summary>
        public async Task<Google.Apis.Drive.v3.Data.File> RenameFileAsync(string fileId, string newName)
        {
            var updateFile = new Google.Apis.Drive.v3.Data.File
            {
                Name = newName
            };

            var updateRequest = _driveService.Files.Update(updateFile, fileId);
            updateRequest.Fields = "id, name, parents";
            var updated = await updateRequest.ExecuteAsync();
            return updated;
        }


        public async Task<string> EnsureResidentFolderAsync(string cedula)
        {
            // 1. Obtener al residente para armar el nombre
            var resident = await _residentService.GetResidentByCedulaAsync(cedula);
            if (resident == null)
                throw new Exception($"No se encontró un residente con cédula {cedula}");

            // 2. Construir el nombre de la carpeta con su nombre completo
            //    Ejemplo: "Juan Carlos Pérez"
            string folderName = $"{resident.Name_RD} {resident.Lastname1_RD} {resident.Lastname2_RD}".Trim();

            // 3. Buscar si la carpeta con ese nombre ya existe en la carpeta padre
            var listRequest = _driveService.Files.List();
            listRequest.Q = $"name = '{folderName}' " +
                            "and mimeType = 'application/vnd.google-apps.folder' " +
                            "and trashed = false " +
                            $"and '{PARENT_FOLDER_ID}' in parents";
            listRequest.Fields = "files(id, name)";

            var result = await listRequest.ExecuteAsync();
            if (result.Files?.Count > 0)
            {
                // Ya existe; devolvemos su Id.
                return result.Files[0].Id;
            }

            // 4. Crear la carpeta con ese nombre dentro de la carpeta padre
            var folderMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = folderName,
                MimeType = "application/vnd.google-apps.folder",
                Parents = new List<string> { PARENT_FOLDER_ID }
            };

            var createRequest = _driveService.Files.Create(folderMetadata);
            createRequest.Fields = "id";
            var folder = await createRequest.ExecuteAsync();

            // 5. Otorgar permisos de lectura pública ("anyone" -> "reader")
            await GrantPublicReadAccessAsync(folder.Id);

            return folder.Id;
        }



    }
}
