using CloudinaryDotNet.Actions;

namespace TodoList.Services.Contracts
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhoto(IFormFile file);
        Task<DeletionResult> DeletePhoto(string publicId);
    }
}
