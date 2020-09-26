using System.IO;
using System.Threading.Tasks;
using ComputerGraphicsApplication.Interfaces;
using ComputerGraphicsApplication.Models.Common;
using File = ComputerGraphicsApplication.Entities.File;

namespace ComputerGraphicsApplication.Services
{
    public class FileService : IService
    {
        public const string FilterImagesStorage = "filter-images";
        public const string ImagesStorage = "images";
        private readonly ApplicationSettings _applicationSettings;

        public FileService(ApplicationSettings applicationSettings)
        {
            _applicationSettings = applicationSettings;
        }

        public async Task<File> UploadFileAsync(UploadFileModel fileModel, string fileName)
        {
            await SaveStreamToFileAsync(fileModel.Stream, fileModel.StorageName, fileName);

            var file = new File
            {
                OriginalFileName = fileModel.FileName,
                Path = Path.Combine(fileModel.StorageName, fileName).Replace('\\', '/'),
                Length = (int) fileModel.Length,
                ContentType = fileModel.ContentType
            };
            return file;
        }

        public async Task SaveStreamToFileAsync(Stream stream, string storageName, string fileName)
        {
            var directoryPath = Path.Combine(_applicationSettings.GetStorageFolder(), storageName);
            Directory.CreateDirectory(directoryPath);

            var filePath = Path.Combine(directoryPath, fileName);
            await using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                stream.Position = 0;
                await stream.CopyToAsync(fileStream);
            }
        }

        public Stream GetStream(string storagePath, string fileName)
        {
            var filePath = Path.Combine(_applicationSettings.GetStorageFolder(), storagePath, fileName);
            return new FileStream(filePath, FileMode.Open);
        }

        public async Task DeleteAsync(string filePath)
        {
            var fullPath = Path.Combine(_applicationSettings.GetStorageFolder(), filePath);
            System.IO.File.Delete(fullPath);
        }
    }
}