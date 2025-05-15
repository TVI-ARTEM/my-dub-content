using Amazon.S3.Model;

namespace MyDub.Content.Repositories;

public interface IS3Repository
{
    Task<string> UploadFileAsync(IFormFile file);
    Task<GetObjectResponse> GetFileAsync(string key);
    Task<string> GetPresignedUrl(string key);
}