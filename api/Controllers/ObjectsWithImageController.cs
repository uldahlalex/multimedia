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
    public async Task<ActionResult<ObjectWithImageResponse>> Upload([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");


        const string bucketName = "easvstorage";

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        using (var stream = file.OpenReadStream())
        {
             await _storageClient.UploadObjectAsync(bucketName, fileName,
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

    [HttpGet]
    [Route("objectsWithImages")]
    public async Task<ActionResult<IEnumerable<ObjectWithImageResponse>>> Get()
    {
        const string bucketName = "easvstorage";
        var objects = _storageClient.ListObjects(bucketName, null);
        var response = new List<ObjectWithImageResponse>();
        foreach (var storageObject in objects)
        {
            response.Add(new ObjectWithImageResponse
            {
                Title = storageObject.Name,
                ImageUrl = $"https://storage.googleapis.com/{bucketName}/{storageObject.Name}"
            });
        }
        return Ok(response);
    }
    
}
