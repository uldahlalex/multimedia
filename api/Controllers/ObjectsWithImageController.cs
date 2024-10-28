using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("")]
public class ObjectsWithImageController : ControllerBase
{
    private readonly ILogger<ObjectsWithImageController> _logger;
    private readonly StorageClient _storageClient;

    public ObjectsWithImageController(ILogger<ObjectsWithImageController> logger)
    {
        _logger = logger;
        _storageClient = StorageClient.Create(GoogleCredential.GetApplicationDefault());
    }

    [HttpPost]
    [Route("objectsWithImages")]
    public async Task<IActionResult> Upload([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");


        const string bucketName = "easvstorage";

        // Generate a unique filename
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        // Upload directly from the IFormFile stream
        using (var stream = file.OpenReadStream())
        {
            // Upload to Google Cloud Storage using the initialized client
            var uploadedObject = await _storageClient.UploadObjectAsync(bucketName, fileName,
                file.ContentType, stream);

            var publicUrl = $"https://storage.googleapis.com/{bucketName}/{fileName}";

            var response = new ObjectWithImageResponse
            {
                Title = fileName,
                ImageUrl = publicUrl
            };

            return Ok(response);
        }
    }
}