using API_LIBRARY.Models;

namespace API_LIBRARY.Interfaces
{
    public interface IImageRepository
    {
        Image Upload(Image image);
        List<Image> GetAllInfoImage();
        (byte[],string,string) Download(int Id);
    }
}
