using JchLegal.Domain.Services;
using Microsoft.Extensions.Configuration;

namespace JchLegal.Infrastructure.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _basePath;
        private readonly string _baseUrl;

        public LocalFileStorageService(IConfiguration configuration)
        {
            _basePath = configuration["FileStorage:BasePath"]
                ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            _baseUrl = configuration["FileStorage:BaseUrl"] ?? "/uploads";
        }

        public async Task<string> SaveAsync(Stream fileStream, string fileName, string mimeType)
        {
            Directory.CreateDirectory(_basePath);

            var uniqueName = $"{Guid.NewGuid()}_{Path.GetFileName(fileName)}";
            var fullPath = Path.Combine(_basePath, uniqueName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await fileStream.CopyToAsync(stream);

            return $"{_baseUrl}/{uniqueName}";
        }

        public Task DeleteAsync(string url)
        {
            var fileName = Path.GetFileName(url);
            var fullPath = Path.Combine(_basePath, fileName);

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            return Task.CompletedTask;
        }
    }
}
