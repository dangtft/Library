using API_LIBRARY.DTO;
using API_LIBRARY.Interfaces;
using API_LIBRARY.Models;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;

namespace API_LIBRARY.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IImageRepository _imageRepository;

        public ImageController(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        [HttpPost]
        [Route("Upload")]
        public IActionResult Upload([FromForm] ImageUploadRequestDTO request)
        {
            ValidateFileUpload(request);
            if (ModelState.IsValid)
            {
                var imageDomainModel = new Image
                {
                    File = request.File,
                    FileDescription = request.FileDescription
                };
                var uploadedImage = _imageRepository.Upload(imageDomainModel);
                return Ok(uploadedImage);
            }
            return BadRequest(ModelState);
        }

        private void ValidateFileUpload(ImageUploadRequestDTO request)
        {
            var allowExtensions = new string[] { ".jpg", ".jpeg", ".png" };
            if (!allowExtensions.Contains(Path.GetExtension(request.File.FileName).ToLower()))
            {
                ModelState.AddModelError("file", "Unsupported file extension");
            }

            if (request.File.Length > 1048576) // 1 MB
            {
                ModelState.AddModelError("file", "File size too big");
            }
        }

        [HttpGet]
        [Route("GetAllInfoImages")]
        public IActionResult GetAllInfoImages()
        {
            var allImages = _imageRepository.GetAllInfoImage();
            return Ok(allImages);
        }

        [HttpGet]
        [Route("Download")]
        public IActionResult DownloadImage(int id)
        {
            try
            {
                var result = _imageRepository.Download(id);
                return File(result.Item1, result.Item2, result.Item3);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

    }
}
