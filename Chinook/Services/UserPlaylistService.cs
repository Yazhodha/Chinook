using Chinook.Models;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services
{
    public class UserPlaylistService : IUserPlaylistService
    {
        private readonly IDbContextFactory<ChinookContext> _dbFactory;
        private Task<ChinookContext> _dbContextTask;
        private readonly INotificationService _notificationService;

        public UserPlaylistService(IDbContextFactory<ChinookContext> dbFactory,
            INotificationService notificationService)
        {
            _dbFactory = dbFactory;
            _dbContextTask = _dbFactory.CreateDbContextAsync();
            _notificationService = notificationService;
        }

        public async void AddToFavourite(long trackId, string currentUserId)
        {
            try
            {
                var dbContext = await _dbContextTask;
                var dbTrack = dbContext.Tracks.FirstOrDefault(t => t.TrackId == trackId);
                var favouritePlayList = dbContext.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == currentUserId && up.Playlist.Name == Constants.Favourite)).FirstOrDefault();

                if (favouritePlayList == null)
                {
                    var playList = await CreateFavouritePlayList(currentUserId);
                    playList.Playlist.Tracks.Add(dbTrack);
                    dbContext.Update(playList);
                }
                else
                {
                    favouritePlayList.Tracks.Add(dbTrack);
                    dbContext.Update(favouritePlayList);
                }

                dbContext.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        private async Task<UserPlaylist> CreateFavouritePlayList(string currentUserId)
        {
            var dbContext = await _dbContextTask;
            var userPlaylist = new UserPlaylist
            {
                UserId = currentUserId,
                Playlist = new Playlist
                {
                    Name = Constants.Favourite,
                    PlaylistId = DateTime.Now.Ticks
                },
            };
            var playList = dbContext.UserPlaylists.Add(userPlaylist);
            dbContext.SaveChanges();

            //Raising the event to update the navigation
            _notificationService.RaisNewPlaylistAdded(userPlaylist.Playlist);

            return playList.Entity;
        }

        public async Task<IQueryable<Playlist>> GetPlaylist(long playListId)
        {
            var dbContext = await _dbContextTask;
            return dbContext.Playlists
                .Include(a => a.Tracks).ThenInclude(a => a.Album).ThenInclude(a => a.Artist)
                .Where(p => p.PlaylistId == playListId);
        }

        public async Task<IQueryable<Playlist>> GetFavouritePlaylist(string currentUserId)
        {
            var dbContext = await _dbContextTask;
            return dbContext.Playlists
                .Include(a => a.Tracks).ThenInclude(a => a.Album).ThenInclude(a => a.Artist)
                .Where(p => p.UserPlaylists.Any(up => up.UserId == currentUserId && up.Playlist.Name == Constants.Favourite));
        }

        public async void RemoveFromFavourite(long trackId, string currentUserId, long playlistId)
        {
            var dbContext = await _dbContextTask;
            var dbTrack = dbContext.Tracks.FirstOrDefault(t => t.TrackId == trackId);
            var favouritePlayList = (await GetPlaylist(playlistId)).FirstOrDefault();

            if (favouritePlayList != null)
            {
                favouritePlayList.Tracks.Remove(dbTrack);
                dbContext.Update(favouritePlayList);
                dbContext.SaveChanges();
            }
        }

        public async void RemoveFromFavourite(long trackId, string currentUserId)
        {
            var dbContext = await _dbContextTask;
            var dbTrack = dbContext.Tracks.FirstOrDefault(t => t.TrackId == trackId);
            var favouritePlayList = (await GetFavouritePlaylist(currentUserId)).FirstOrDefault();

            if (favouritePlayList != null)
            {
                favouritePlayList.Tracks.Remove(dbTrack);
                dbContext.Update(favouritePlayList);
                dbContext.SaveChanges();
            }
        }

        public async void RemoveFromPlaylist(long trackId, string currentUserId, long playlistId)
        {
            var dbContext = await _dbContextTask;
            var dbTrack = dbContext.Tracks.FirstOrDefault(t => t.TrackId == trackId);
            var playlist = (await GetPlaylist(playlistId)).FirstOrDefault();

            if (playlist != null)
            {
                playlist.Tracks.Remove(dbTrack);
                dbContext.Update(playlist);
                dbContext.SaveChanges();
            }
        }

        public async Task<List<Playlist>> GetPlaylistsByUserId(string currentUserId)
        {
            var dbContext = await _dbContextTask;
            return dbContext.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == currentUserId)).ToList();
        }

        public async void AddPlaylist(string playlistName, string currentUserId, long trackId)
        {
            var dbContext = await _dbContextTask;
            var dbTrack = dbContext.Tracks.FirstOrDefault(t => t.TrackId == trackId);

            //check for existing playlist and creat new if not
            var existingPlaylist = dbContext.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == currentUserId && up.Playlist.Name == playlistName)).SingleOrDefault();

            if (existingPlaylist != null)
            {
                existingPlaylist.Tracks.Add(dbTrack);
                dbContext.Update(existingPlaylist);
                dbContext.SaveChanges();
            }
            else
            {
                var userPlaylist = new UserPlaylist
                {
                    UserId = currentUserId,
                    PlaylistId = DateTime.Now.Ticks,
                    Playlist = new Playlist
                    {
                        Name = playlistName,
                        PlaylistId = DateTime.Now.Ticks,
                        Tracks = new List<Track> { dbTrack }
                    },
                };
                var playlist = dbContext.UserPlaylists.Add(userPlaylist);

                dbContext.SaveChanges();

                //Raising the event to update the navigation
                _notificationService.RaisNewPlaylistAdded(userPlaylist.Playlist);
            }
        }

        public async Task<List<Playlist>> GetAllUserPlaylist()
        {
            var dbContext = await _dbContextTask;
            return dbContext.Playlists.ToList();
        }
    }
}
