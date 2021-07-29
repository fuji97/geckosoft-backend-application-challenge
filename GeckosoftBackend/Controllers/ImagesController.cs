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

        [HttpGet()]
        public async Task<ActionResult> GetAllImages() {
            var allImages = _imagesManager.GetAllImages();
            return Ok(allImages);
        }

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