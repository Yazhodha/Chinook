using Chinook.Models;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services
{
    public class AlbumService : IAlbumService
    {
        private IDbContextFactory<ChinookContext> _dbFactory;
        private Task<ChinookContext> _dbContextTask;

        public AlbumService(IDbContextFactory<ChinookContext> dbFactory)
        {
            _dbFactory = dbFactory;
            _dbContextTask = _dbFactory.CreateDbContextAsync();
        }

        public async Task<List<Album>> GetAlbumsForArtist(int artistId)
        {
            var dbContext = await _dbContextTask;
            return dbContext.Albums.Where(a => a.ArtistId == artistId).ToList();
        }
    }
}
