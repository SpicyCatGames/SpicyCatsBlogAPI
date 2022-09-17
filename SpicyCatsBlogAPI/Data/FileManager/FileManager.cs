using PhotoSauce.MagicScaler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpicyCatsBlogAPI.Data.FileManager
{
    public class FileManager : IFileManager
    {
        private readonly string _imagePath;

        public FileManager(IConfiguration config)
        {
            _imagePath = config["Path:Images"];
        }

        public FileStream ImageStream(string image)
        {
            return new FileStream(Path.Combine(_imagePath, image), FileMode.Open, FileAccess.Read);
        }

        public async Task<string> SaveImage(IFormFile image)
        {
            try
            {
                var save_path = Path.Combine(_imagePath);
                if (!Directory.Exists(save_path))
                {
                    Directory.CreateDirectory(save_path);
                }

                //internet explorer error
                //var fileName = image.FileName;
                // var mime = image.FileName.Substring(image.FileName.LastIndexOf('.'));
                var mime = ".jpg";
                var fileName = $"img_{DateTime.Now.ToString("dd-MM-yy-HH-mm-ss")}{mime}";

                using (var fileStream = new FileStream(Path.Combine(save_path, fileName), FileMode.Create))
                {
                    // await image.CopyToAsync(fileStream);
                    await Task.Run(() =>
                    {
                        MagicImageProcessor.ProcessImage(image.OpenReadStream(), fileStream, ImageOptions());
                    });
                }

                return fileName;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return "Error";
            }
        }

        public bool RemoveImage(string image)
        {
            try
            {
                var file = Path.Combine(_imagePath, image);
                if (File.Exists(file))
                {
                    File.Delete(file);
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        private static ProcessImageSettings ImageOptions()
        {
            var settings = new ProcessImageSettings()
            {
                Width = 800,
                Height = 500,
                ResizeMode = CropScaleMode.Crop,
                // EncoderOptions = new JpegEncoderOptions(100, ChromaSubsampleMode.Subsample420, true),
            };
            settings.TrySetEncoderFormat(ImageMimeTypes.Jpeg);
            return settings;
        }
    }
}