namespace SE1611_Group1_Project.FileUploadService
{
    public interface IFileUploadService
    {
        Task<string>  UploadFileAsync(IFormFile file);
    }
}
