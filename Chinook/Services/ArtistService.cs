using Chinook.Models;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services
{
    public class ArtistService : IArtistService
    {
        private IDbContextFactory<ChinookContext> _dbFactory;
        private Task<ChinookContext> _dbContextTask;

        public ArtistService(IDbContextFactory<ChinookContext> dbFactory)
        {
            _dbFactory = dbFactory;
            _dbContextTask = _dbFactory.CreateDbContextAsync();
        }

        public async Task<List<Track>> GetAlbumTracks(long artistId)
        {
            var dbContext = await _dbContextTask;
            return dbContext.Tracks.Where(a => a.Album.ArtistId == artistId)
                .Include(a => a.Album).Include(p => p.Playlists).ThenInclude(up => up.UserPlaylists).ToList();
        }

        public async Task<Artist> GetArtistById(long artistId)
        {
            var dbContext = await _dbContextTask;
            return dbContext.Artists.SingleOrDefault(a => a.ArtistId == artistId);
        }

        public async Task<List<Artist>> GetArtists()
        {
            var dbContext = await _dbContextTask;
            var users = dbContext.Users.Include(a => a.UserPlaylists).ToList();

            return dbContext.Artists.ToList();
        }
    }
}
