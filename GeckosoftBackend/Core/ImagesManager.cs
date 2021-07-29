using System;
using System.IO;
using System.Threading.Tasks;
using GeckosoftBackend.Models;
using Microsoft.AspNetCore.Http;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace GeckosoftBackend.Core {
    public class ImagesManager : IImagesManager {
        private IConfiguration _configuration;
        private IHttpClientFactory _httpClientFactory;
        private string _baseImagePath;
        private Uri _baseUrl;

        public ImagesManager(IConfiguration configuration, IHttpClientFactory httpClientFactory) {
            _configuration = configuration;
            _baseImagePath = _configuration["ImageStorage"];
            _baseUrl = new Uri(_configuration["BaseUrlImage"]);
            _httpClientFactory = httpClientFactory;
        }

        public ImageModel[] GetAllImages() {
            var files = Directory.GetFiles(_baseImagePath).Select(Path.GetFileName);
            return files.Select(x => new ImageModel(x, _baseUrl))
                .OrderBy(x => x.Name)
                .ToArray();
        }

        public ImageModel DeleteImage(string name) {
            File.Delete(Path.Combine(_baseImagePath, name));
            return new ImageModel(name, _baseUrl);
        }

        public async Task<ImageModel> ResizeImage(string name, int width, int height, Uri callback) {
            if (!FileExists(name) || width <= 0 || height <= 0) {
                throw new ArgumentException("File is missing or width or height is negative");
            }

            var model = new ImageModel(name, _baseUrl);

            if (callback != null) {
                Task.Run(() => ResizeAndCallback(model, width, height, callback));
            } else {
                await ResizeAndCallback(model, width, height, null);
            }

            return model;
        }

        public async Task<ImageModel> AddImage(IFormFile file, string name) {
            var filepath = Path.Combine(_baseImagePath, name);
            await WriteImage(file, filepath.ToString());

            return new ImageModel() {
                BaseUrl = _baseUrl,
                Name = name
            };
        }

        public bool FileExists(string name) {
            return Directory.GetFiles(_baseImagePath).Any(x => name == Path.GetFileName(x));
        }
        
        private async Task WriteImage(IFormFile file, string filepath) {
            try {
                await using FileStream filestream = System.IO.File.Create(filepath);
                await file.CopyToAsync(filestream);
                filestream.Flush();
            } catch (Exception ex) {
                // TODO Log Error
                throw;
            }
        }

        private async Task ResizeAndCallback(ImageModel model, int width, int height, Uri callback) {
            var path = Path.Combine(_baseImagePath, model.Name);
            var image = await Image.LoadAsync(path);
            image.Mutate(x => x.Resize(width, height));
            await image.SaveAsync(path);

            if (callback != null)
                await PerformCallback(model, callback);
        }

        private async Task PerformCallback(ImageModel model, Uri callback) {
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, callback);
            request.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(model));

            await client.SendAsync(request);
        }
    }
}