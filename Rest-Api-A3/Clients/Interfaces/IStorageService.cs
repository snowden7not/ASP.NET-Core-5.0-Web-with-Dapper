using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public interface IStorageService
{
    Task<string> UploadMoviePosterAsync(int movieId, IFormFile file);

    Task DeleteMoviePosterByUrlAsync(string publicUrl);
}
