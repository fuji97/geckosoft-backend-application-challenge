using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GeckosoftBackend.Core;
using GeckosoftBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using Xunit;

namespace GeckosoftBackend.Tests {
    public class TestImagesManager {
        private static readonly string FILE_NAME = "image.png";
        private static readonly string FILE_PATH = $"static/{FILE_NAME}";
        
        
        private ImagesManager _imagesManager;
        
        private static readonly Dictionary<string, string> SETTINGS = new Dictionary<string, string> {
            {"ImageStorage", "wwwroot/files/"},
            {"BaseUrlImage", "http://localhost:5000/files/"},
        };

        private IConfiguration _configuration;

        public TestImagesManager() {
            
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(SETTINGS)
                .Build();
            _imagesManager = new ImagesManager(_configuration, null);
            
             // Clear wwwroot/files
             var di = new DirectoryInfo(SETTINGS["ImageStorage"]);
             foreach (FileInfo file in di.GetFiles()) {
                 if (file.Name != "ignore") // Keeping one element to keep folder in build, searching better solutions
                    file.Delete(); 
             }
        }

        [Fact]
        public async Task TestAddImage() {
            // Prepare
            var image = new ImageModel(FILE_NAME, new Uri(SETTINGS["BaseUrlImage"]));
            using var stream = new MemoryStream((await File.ReadAllBytesAsync(FILE_PATH)).ToArray());
            var formFile = new FormFile(stream, 0, stream.Length, "file", FILE_NAME);
            
            // Act
            var res = await _imagesManager.AddImage(formFile, FILE_NAME);
            
            // Assert
            Assert.Equal(image, res);
        }
        
        [Fact]
        public async Task TestRemoveImage() {
            // Prepare
            var image = new ImageModel(FILE_NAME, new Uri(SETTINGS["BaseUrlImage"]));
            File.Copy(FILE_PATH, Path.Combine(SETTINGS["ImageStorage"], FILE_NAME));
            
            // Act
            var res = _imagesManager.DeleteImage(FILE_NAME);
            
            // Assert
            Assert.Equal(image, res);
        }
        
        [Fact]
        public async Task TestImageExists() {
            // Prepare
            var image = new ImageModel(FILE_NAME, new Uri(SETTINGS["BaseUrlImage"]));
            File.Copy(FILE_PATH, Path.Combine(SETTINGS["ImageStorage"], FILE_NAME));
            
            // Act
            var res = _imagesManager.FileExists(FILE_NAME);
            
            // Assert
            Assert.True(res);
        }
        
        [Fact]
        public async Task TestResizeImage() {
            // Prepare
            var width = 100;
            var height = 90;
            var image = new ImageModel(FILE_NAME, new Uri(SETTINGS["BaseUrlImage"]));
            File.Copy(FILE_PATH, Path.Combine(SETTINGS["ImageStorage"], FILE_NAME));
            
            // Act
            var res = await _imagesManager.ResizeImage(FILE_NAME, width, height, null);
            
            // Assert
            Assert.Equal(image, res);
            var imageData = await Image.LoadAsync(Path.Combine(SETTINGS["ImageStorage"], FILE_NAME));
            Assert.Equal(width, imageData.Width);
            Assert.Equal(height, imageData.Height);
        }
        
        [Fact]
        public async Task TestGetAllImages() {
            // Prepare
            var images = new List<ImageModel>() {
                new ImageModel("image1.png", new Uri(SETTINGS["BaseUrlImage"])),
                new ImageModel("image2.png", new Uri(SETTINGS["BaseUrlImage"]))
            };
            File.Copy(FILE_PATH, Path.Combine(SETTINGS["ImageStorage"], images[0].Name));
            File.Copy(FILE_PATH, Path.Combine(SETTINGS["ImageStorage"], images[1].Name));
            
            // Act
            var res = _imagesManager.GetAllImages();
            
            // Assert
            Assert.Equal(images, res.Where(x => x.Name != "ignore"));
        }
    }
}