using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Demo1.Controllers {
    [ApiController]
    [Route("api/v{version:apiVersion}/files")]
    public class FilesController : ControllerBase {
        private FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;
        private ILogger<FilesController> _logger;

        public FilesController(
            FileExtensionContentTypeProvider fileExtensionContentTypeProvider, 
            ILogger<FilesController> logger) {


            _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("{name}")]
        public ActionResult GetFile(string name) {
            var path = Path.Combine("Files", name);

            if (!System.IO.File.Exists(path)) {
                return NotFound();
            }

            var data = System.IO.File.ReadAllBytes(path);

            if (!_fileExtensionContentTypeProvider.TryGetContentType(path, out var mimeType)) {
                mimeType = "application/octet-stream";
            }

            _logger.LogInformation($"Returned file with name {name} and mime type {mimeType}.");

            return File(data, mimeType, name);
        }

        [HttpPost]
        public async Task<ActionResult> UploadFile(IFormFile file) {
            if (file.Length > 1000000) { // 1MB
                return BadRequest("File too big");
            }
            if (file.ContentType != "application/pdf") {
                return BadRequest("I only accept pdf files");
            }

            string path = Path.Combine("uploads", Guid.NewGuid() + $"_{file.FileName}");

            using (var stream = new FileStream(path, FileMode.Create)) {
                await file.CopyToAsync(stream);
            }

            return NoContent();
        }
    }
}
