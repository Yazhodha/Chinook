using Chinook.Models;

namespace Chinook.Services
{
    public interface IUserPlaylistService
    {
        void AddToFavourite(long trackId, string currentUserId);
        Task<IQueryable<Playlist>> GetPlaylist(long id);
    }
}
