using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Image_different_sizes.Controllers
{
    public class ImageController : Controller
    {
        private readonly IWebHostEnvironment _environment;

        public ImageController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public IActionResult Uploadpicture()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("file not selected");

            var uploadFolder = Path.Combine(_environment.WebRootPath, "uploads");
            var imageName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var imagePath = Path.Combine(uploadFolder, imageName);
            Directory.CreateDirectory(uploadFolder);

            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            var originalImagePath = imagePath;
            var imagePaths = new Dictionary<string, string>();
            imagePaths["small"] = ResizeAndSaveImage(originalImagePath, "small", 100);
            imagePaths["medium"] = ResizeAndSaveImage(originalImagePath, "medium", 300);
            imagePaths["large"] = ResizeAndSaveImage(originalImagePath, "large", 600);

            TempData["ImagePaths"] = imagePaths;

            return RedirectToAction("Uploadpicture");
        }

        private string ResizeAndSaveImage(string originalImagePath, string suffix, int size)
        {
            var resizedImagePath = Path.Combine(_environment.WebRootPath, $"uploads/{suffix}-{Path.GetFileName(originalImagePath)}");

            using (var image = Image.Load(originalImagePath))
            {
                image.Mutate(x => x.Resize(size, size));
                image.Save(resizedImagePath);
            }

            return Path.Combine("/uploads/", $"{suffix}-{Path.GetFileName(originalImagePath)}");
        }
    }
}
