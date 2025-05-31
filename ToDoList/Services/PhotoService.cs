using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using TodoList.DTOs.CloudinarySettings;
using TodoList.Services.Contracts;

namespace TodoList.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        public PhotoService(IOptions<CloudinarySettings> config)
        {
            var account = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);

            _cloudinary = new Cloudinary(account);
        }
        public async Task<ImageUploadResult> AddPhoto(IFormFile file)
        {
            var uploadImage = new ImageUploadResult();

            if(file.Length > 0)
            {
               using var stream = file.OpenReadStream();
                var photoUploadParameters = new ImageUploadParams
                {
                   File = new FileDescription(file.FileName, stream),
                   Transformation = new Transformation().Height(Constants.PHOTO_HEIGHT).Width(Constants.PHOTO_WIDTH).Crop("fill").Gravity("face"),
                   Folder = Constants.CLOUDINARY_FOLDER_NAME
                };

                uploadImage = await _cloudinary.UploadAsync(photoUploadParameters);
            }

            return uploadImage;
        }

        public async Task<DeletionResult> DeletePhoto(string publicId)
        {
            var deletePhotoParams = new DeletionParams(publicId);

            return await _cloudinary.DestroyAsync(deletePhotoParams);
        }
    }
}
