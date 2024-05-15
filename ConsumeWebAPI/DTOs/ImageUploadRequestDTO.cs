namespace ConsumeWebAPI.DTOs
{
    public class ImageUploadRequestDTO
    {
        public IFormFile File { get; set; }
        public string FileName { get; set; }
        public string FileDescription { get; set; }
    }
}
