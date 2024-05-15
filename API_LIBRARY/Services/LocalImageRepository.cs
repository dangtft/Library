using API_LIBRARY.Data;
using API_LIBRARY.Interfaces;
using API_LIBRARY.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace API_LIBRARY.Services
{
    public class LocalImageRepository : IImageRepository
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LibaryDbContext _dbContext;

        public LocalImageRepository(IHttpContextAccessor httpContextAccessor, LibaryDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        public Image Upload(Image image)
        {
            // Generate unique file name
            image.FileName = Guid.NewGuid().ToString();
            image.FileExtension = Path.GetExtension(image.File.FileName);

            var localFilePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images", $"{image.FileName}{image.FileExtension}");

            using (var stream = new FileStream(localFilePath, FileMode.Create))
            {
                image.File.CopyTo(stream);
            }

            var urlFilePath = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}/Images/{image.FileName}{image.FileExtension}";
            image.FilePath = urlFilePath;

            image.FileSizeInBytes = image.File.Length;

            _dbContext.Images.Add(image);
            _dbContext.SaveChanges();

            return image;
        }

        public List<Image> GetAllInfoImage()
        {
            return _dbContext.Images.ToList();
        }

        public (byte[], string, string) Download(int Id)
        {
            try
            {
                var fileById = _dbContext.Images.FirstOrDefault(x => x.Id == Id);
                if (fileById == null)
                {
                    throw new FileNotFoundException("File not found in the database");
                }

                var path = Path.Combine(_webHostEnvironment.ContentRootPath, "Images", $"{fileById.FileName}{fileById.FileExtension}");
                if (!System.IO.File.Exists(path))
                {
                    throw new FileNotFoundException("File not found on the server");
                }

                var stream = System.IO.File.ReadAllBytes(path);
                var fileName = fileById.FileName + fileById.FileExtension;

                return (stream, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                throw new Exception("An error occurred while downloading the file.", ex);
            }
        }

    }
}
