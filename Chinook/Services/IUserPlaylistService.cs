using Chinook.Models;

namespace Chinook.Services
{
    public interface IUserPlaylistService
    {
        void AddToFavourite(long trackId, string currentUserId);
        Task<IQueryable<Playlist>> GetPlaylist(long id);
        void RemoveFromFavourite(long trackId, string currentUserId, long playlistId);
        void RemoveFromPlaylist(long trackId, string currentUserId, long playlistId);
    }
}
