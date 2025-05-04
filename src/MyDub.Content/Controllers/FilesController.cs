using Amazon.S3;
using Microsoft.AspNetCore.Mvc;
using MyDub.Content.Repositories;

namespace MyDub.Content.Controllers;

[ApiController]
[Route("api")]
public class FilesController(IS3Repository s3Repository) : ControllerBase
{
    [HttpPost("upload")]
    [RequestSizeLimit(257_000_000)]
    public async Task<IActionResult> Upload(IFormFile? file)
    {
        if (file is null)
            return BadRequest("File is required");

        var key = await s3Repository.UploadFileAsync(file);
        return Ok(new { key });
    }

    [HttpGet("{key}")]
    public async Task<IActionResult> Download(string key)
    {
        try
        {
            var response = await s3Repository.GetFileAsync(key);
            return File(response.ResponseStream, response.Headers.ContentType, key);
        }
        catch (AmazonS3Exception e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return NotFound();
        }
    }
}