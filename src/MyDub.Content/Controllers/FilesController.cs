using Amazon.S3;
using Microsoft.AspNetCore.Mvc;
using MimeTypes;
using MyDub.Content.Repositories;

namespace MyDub.Content.Controllers;

[ApiController]
[Route("api")]
public class FilesController(IS3Repository s3Repository) : ControllerBase
{
    [HttpPost("upload")]
    [RequestSizeLimit(257_000_000)]
    public async Task<ActionResult<string>> Upload(IFormFile? file)
    {
        if (file is null)
            return BadRequest("File is required");

        var key = await s3Repository.UploadFileAsync(file);
        return Ok(key);
    }

    [HttpGet("{key}")]
    public async Task<IResult> Download(string key)
    {
        try
        {
            var response = await s3Repository.GetFileAsync(key);
            return Results.File(response.ResponseStream, response.Headers.ContentType, key + MimeTypeMap.GetExtension(response.Headers.ContentType));
        }
        catch (AmazonS3Exception e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return Results.NotFound();
        }
    }
}