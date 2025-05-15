using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using MyDub.Content.Configure;

namespace MyDub.Content.Repositories;

public class S3Repository(IAmazonS3 s3Client, IOptions<S3Settings> options) : IS3Repository
{
    private readonly S3Settings _settings = options.Value;

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        try
        {
            await s3Client.EnsureBucketExistsAsync(_settings.BucketName);
        }
        catch (Exception)
        {
            // ignored
        }

        var key = Guid.NewGuid().ToString();
        await using var stream = file.OpenReadStream();

        var request = new PutObjectRequest
        {
            BucketName = _settings.BucketName,
            Key = key,
            InputStream = stream,
            ContentType = file.ContentType
        };
        await s3Client.PutObjectAsync(request);
        return key;
    }

    public async Task<GetObjectResponse> GetFileAsync(string key)
    {
        try
        {
            await s3Client.EnsureBucketExistsAsync(_settings.BucketName);
        }
        catch (Exception)
        {
            // ignored
        }

        var request = new GetObjectRequest
        {
            BucketName = _settings.BucketName,
            Key = key
        };

        return await s3Client.GetObjectAsync(request);
    }

    public async Task<string> GetPresignedUrl(string key)
    {
        try
        {
            await s3Client.EnsureBucketExistsAsync(_settings.BucketName);
        }
        catch (Exception)
        {
            // ignored
        }

        var x = await s3Client.GetObjectMetadataAsync(new GetObjectMetadataRequest
        {
            BucketName = _settings.BucketName,
            Key = key,
        });

        var request = new GetPreSignedUrlRequest
        {
            BucketName = _settings.BucketName,
            Key = key,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.AddDays(2),
            Protocol = Protocol.HTTP,
            ResponseHeaderOverrides = new ResponseHeaderOverrides
            {
                ContentType = x.Headers.ContentType
            }
        };

        return await s3Client.GetPreSignedURLAsync(request);
    }
}