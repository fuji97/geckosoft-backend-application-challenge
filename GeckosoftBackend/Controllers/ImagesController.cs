using System;
using System.IO;
using System.Threading.Tasks;
using GeckosoftBackend.Core;
using GeckosoftBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace GeckosoftBackend.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class ImagesController : ControllerBase {
        private IConfiguration _configuration;
        private IImagesManager _imagesManager;

        public ImagesController(IConfiguration configuration, IImagesManager imagesManager) {
            _configuration = configuration;
            _imagesManager = imagesManager;
        }

        public class FIleUploadAPI
        {
            public IFormFile File { get; set; }
        }

        /// <summary>
        /// Upload an image.
        /// </summary>
        /// <param name="file">File to upload</param>
        /// <returns>Name and URL of the uploaded image</returns>
        [HttpPost()]
        public async Task<ActionResult> UploadImage([FromForm] FIleUploadAPI file) {
            if (file.File.Length > 0) {
                try {
                    return Ok(await _imagesManager.AddImage(file.File, file.File.FileName));
                } catch (Exception e) {
                    return Problem(e.ToString());
                }
            } else {
                return BadRequest();
            }
        }

        /// <summary>
        /// Get the alphabetical sorted list of all uploaded images.
        /// </summary>
        /// <returns>List of name and URL of the images.</returns>
        [HttpGet()]
        public async Task<ActionResult> GetAllImages() {
            var allImages = _imagesManager.GetAllImages();
            return Ok(allImages);
        }

        /// <summary>
        /// Delete an uploaded image.
        /// </summary>
        /// <param name="name">Name of the image to delete.</param>
        /// <returns>Name and URL of the uploaded image.</returns>
        [HttpDelete("{name}")]
        public async Task<ActionResult> DeleteImage(string name) {
            if (!_imagesManager.FileExists(name)) {
                return BadRequest(new {
                    errror = $"Image '{name}' not found."
                });
            }

            var result = _imagesManager.DeleteImage(name);
            return Ok(result);
        }

        /// <summary>
        /// Resize an image. If passing a callback URL, the URL will be called after the completion of the operation.
        /// If callbackUrl is left null the operation will be synchronous.
        /// </summary>
        /// <param name="name">Name of the image to resize</param>
        /// <param name="width">New width of the image in pixels</param>
        /// <param name="height">New height of the image in pixels</param>
        /// <param name="callbackUrl">Callback to call after the operation</param>
        /// <returns>Name and URl of the resized image</returns>
        [HttpPut]
        public async Task<ActionResult> ResizeImage([FromForm] string name, 
            [FromForm] int width, 
            [FromForm] int height,
            [FromForm] string callbackUrl = null) {
            Uri callback = null;

            if (width < 1 || height < 1) {
                return BadRequest("Width and height can't be less than 1.");
            }

            if (!_imagesManager.FileExists(name)) {
                return BadRequest($"File '{name}' not found.");
            }
            
            if (callbackUrl != null) {
                try {
                    callback = new Uri(callbackUrl);
                } catch (Exception e) {
                    return BadRequest($"'{callbackUrl}' is not a valid callback URL.");
                }
            }

            try {
                var res = await _imagesManager.ResizeImage(name, width, height, callback);
                return Ok(res);
            } catch (ArgumentException e) {
                return BadRequest( e.ToString());
            }
        }
    }
}