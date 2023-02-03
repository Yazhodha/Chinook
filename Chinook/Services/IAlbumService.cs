using Chinook.Models;

namespace Chinook.Services
{
    public interface IAlbumService
    {
        Task<List<Album>> GetAlbumsForArtist(int artistId);
    }
}
