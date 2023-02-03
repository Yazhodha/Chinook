using Chinook.Models;

namespace Chinook.Services
{
    public interface IUserPlaylistService
    {
        Task<IQueryable<Playlist>> GetPlaylist(long id);
    }
}
