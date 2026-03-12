namespace JchLegal.Domain.Services
{
    public interface IFileStorageService
    {
        Task<string> SaveAsync(Stream fileStream, string fileName, string mimeType);
        Task DeleteAsync(string url);
    }
}
