using Chinook.Models;

namespace Chinook.Services
{
    public interface IUserPlaylistService
    {
        void AddToFavourite(long trackId, string currentUserId);
        void AddPlaylist(string newPlaylistName, string currentUserId, long trackId);
        Task<IQueryable<Playlist>> GetPlaylist(long id);
        Task<List<Playlist>> GetPlaylistsByUserId(string currentUserId);
        void RemoveFromFavourite(long trackId, string currentUserId, long playlistId);
        void RemoveFromPlaylist(long trackId, string currentUserId, long playlistId);
        Task<List<Playlist>> GetAllUserPlaylist();
    }
}
