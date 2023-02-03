using Chinook.Models;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services
{
    public class UserPlaylistService : IUserPlaylistService
    {
        private readonly IDbContextFactory<ChinookContext> _dbFactory;
        private Task<ChinookContext> _dbContextTask;

        public UserPlaylistService(IDbContextFactory<ChinookContext> dbFactory)
        {
            _dbFactory = dbFactory;
            _dbContextTask = _dbFactory.CreateDbContextAsync();
        }

        public async Task<IQueryable<Playlist>> GetPlaylist(long playListId)
        {
            var dbContext = await _dbContextTask;
            return dbContext.Playlists
                .Include(a => a.Tracks).ThenInclude(a => a.Album).ThenInclude(a => a.Artist)
                .Where(p => p.PlaylistId == playListId);
        }
    }
}
