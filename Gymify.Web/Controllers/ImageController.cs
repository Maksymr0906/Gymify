using FluentValidation;
using Gymify.Application.DTOs.Image;
using Gymify.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gymify.Web.Controllers
{
    public class ImageController : Controller
    {
        private readonly IImageService _imageService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImageController(IImageService imageService, IWebHostEnvironment webHostEnvironment)
        {
            _imageService = imageService;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: /Image
        public async Task<IActionResult> Index()
        {
            var images = await _imageService.GetAllImagesAsync();
            return View(images);
        }

        // GET: /Image/Upload
        public IActionResult Upload()
        {
            return View();
        }

        // POST: /Image/Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file, string fileName, string title)
        {
            if (file == null || string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(title))
            {
                ModelState.AddModelError(string.Empty, "Please fill in all fields and choose a file.");
                return View();
            }

            try
            {
                var fileExtension = Path.GetExtension(file.FileName);
                var urlPath = $"/Images/{fileName}{fileExtension}";
                var localPath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", $"{fileName}{fileExtension}");

                Directory.CreateDirectory(Path.GetDirectoryName(localPath)!);

                await using (var stream = new FileStream(localPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var imageUploadModel = new ImageUploadModel(default, default, default, default, default, default)
                {
                    FileName = fileName,
                    FileExtension = fileExtension,
                    Title = title,
                    LocalPath = localPath,
                    UrlPath = urlPath,
                    FileContent = await ConvertFileToByteArrayAsync(file)
                };

                await _imageService.CreateImageAsync(imageUploadModel);

                TempData["SuccessMessage"] = "Image uploaded successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View();
            }
        }

        private async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
    }
}
