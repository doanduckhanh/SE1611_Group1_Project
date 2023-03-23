namespace SE1611_Group1_A3.FileUploadService
{
    public interface IFileUploadService
    {
        Task<string>  UploadFileAsync(IFormFile file);
    }
}
