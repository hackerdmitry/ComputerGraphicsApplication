using System.IO;

namespace ComputerGraphicsApplication.Models.Common
{
    public class ApplicationSettings
    {
        public string StorageRoot { get; set; }
        public string Files { get; set; }

        public string GetStorageFolder(bool includeRoot = true)
        {
            return includeRoot
                       ? Path.Combine(StorageRoot, Files)
                       : $"/{Files}";
        }
    }
}