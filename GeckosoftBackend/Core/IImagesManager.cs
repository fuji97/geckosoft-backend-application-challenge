using System;
using System.Threading.Tasks;
using GeckosoftBackend.Models;
using Microsoft.AspNetCore.Http;

namespace GeckosoftBackend.Core {
    public interface IImagesManager {
        ImageModel[] GetAllImages();
        ImageModel DeleteImage(string name);
        Task<ImageModel> ResizeImage(string name, int width, int height, Uri callback);
        Task<ImageModel> AddImage(IFormFile data, string name);
        bool FileExists(string name);
    }
}