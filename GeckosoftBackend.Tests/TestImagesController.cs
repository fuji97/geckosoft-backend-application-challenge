using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeckosoftBackend.Controllers;
using GeckosoftBackend.Core;
using GeckosoftBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using SixLabors.ImageSharp;
using Xunit;

namespace GeckosoftBackend.Tests {
    public class TestImagesController {
        public static string FAKE_URL = "http://fakeurl.it/file.png";
        
        private static Dictionary<string, string> settings = new Dictionary<string, string> {
            {"ImageStorage", "wwwroot/files/"},
            {"BaseUrlImage", "http://localhost:5000/files/"},
        };

        private IConfiguration configuration;

        public TestImagesController() {
            configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();
        }

        [Fact]
        public async Task TestGetAllImages() {
            //Arrange
            var images = new List<ImageModel>() {
                new ImageModel("image1.png", new Uri(FAKE_URL)),
                new ImageModel("image2.png", new Uri(FAKE_URL)),
                new ImageModel("image3.png", new Uri(FAKE_URL)),
            };

            // Mocks
            var mockImagesManager = new Mock<IImagesManager>();
            mockImagesManager.Setup(x => x.GetAllImages())
                .Returns(images.ToArray());
            
            //Act
            var imagesController = new ImagesController(
                configuration,
                mockImagesManager.Object);
            
            var actionResult = await imagesController.GetAllImages();

            //Assert
            var res = Assert.IsType<OkObjectResult>(actionResult);
            var model = Assert.IsType<ImageModel[]>(res.Value);
            Assert.Equal(images, model);
            mockImagesManager.Verify();
        }
        
        [Fact]
        public async Task TestAddImage() {
            //Arrange
            var image = new ImageModel("image3.png", new Uri(FAKE_URL));

            // Mocks
            var mockIFormFile = new Mock<IFormFile>();
            mockIFormFile.Setup(x => x.FileName)
                .Returns(image.Name);
            mockIFormFile.Setup(x => x.Length).Returns(1);
            
            var mockImagesManager = new Mock<IImagesManager>();
            mockImagesManager.Setup(x => x.AddImage(mockIFormFile.Object, image.Name))
                .Returns(Task.FromResult(image));
            
            //Act
            var imagesController = new ImagesController(
                configuration,
                mockImagesManager.Object);

            var fIleUploadApi = new ImagesController.FIleUploadAPI() {File = mockIFormFile.Object};
            var actionResult = await imagesController.UploadImage(fIleUploadApi);

            //Assert
            var res = Assert.IsType<OkObjectResult>(actionResult);
            var model = Assert.IsType<ImageModel>(res.Value);
            Assert.Equal(image, model);
            mockImagesManager.Verify();
            mockIFormFile.Verify();
        }
        
        [Fact]
        public async Task TestDeleteImage() {
            //Arrange
            var image = new ImageModel("image3.png", new Uri(FAKE_URL));

            // Mocks

            var mockImagesManager = new Mock<IImagesManager>();
            mockImagesManager.Setup(x => x.DeleteImage(image.Name))
                .Returns(image);
            mockImagesManager.Setup(x => x.FileExists(image.Name))
                .Returns(true);
            
            //Act
            var imagesController = new ImagesController(
                configuration,
                mockImagesManager.Object);
            
            var actionResult = await imagesController.DeleteImage(image.Name);

            //Assert
            var res = Assert.IsType<OkObjectResult>(actionResult);
            var model = Assert.IsType<ImageModel>(res.Value);
            Assert.Equal(image, model);
            mockImagesManager.Verify();
        }
        
        [Fact]
        public async Task TestResizeImage() {
            //Arrange
            var image = new ImageModel("image3.png", new Uri(FAKE_URL));
            var width = 100;
            var height = 90;

            // Mocks
            var mockImagesManager = new Mock<IImagesManager>();
            mockImagesManager.Setup(x => x.ResizeImage(image.Name, width, height, null))
                .Returns(Task.FromResult(image));
            mockImagesManager.Setup(x => x.FileExists(image.Name))
                .Returns(true);
            
            //Act
            var imagesController = new ImagesController(
                configuration,
                mockImagesManager.Object);
            
            var actionResult = await imagesController.ResizeImage(image.Name, width, height, null);

            //Assert
            var res = Assert.IsType<OkObjectResult>(actionResult);
            var model = Assert.IsType<ImageModel>(res.Value);
            Assert.Equal(image, model);
            mockImagesManager.Verify();
        }
    }
}