using Chinook.ClientModels;
using Chinook.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Chinook.Pages
{
    public class PlaylistPageBase : ComponentBase
    {
        [Parameter] public long PlaylistId { get; set; }
        [Inject] IUserPlaylistService _userPlaylistService { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationState { get; set; }

        protected ClientModels.Playlist Playlist;
        protected string CurrentUserId;
        protected string InfoMessage;

        protected override async Task OnInitializedAsync()
        {
            CurrentUserId = await GetUserId();

            await InvokeAsync(StateHasChanged);
            await LoadPlaylist();
        }

        private async Task LoadPlaylist()
        {
            var playList = await _userPlaylistService.GetPlaylist(PlaylistId);
            if (playList != null)
            {
                Playlist = playList.Select(p => new ClientModels.Playlist()
                {
                    Name = p.Name,
                    Tracks = p.Tracks.Select(t => new PlaylistTrack()
                    {
                        AlbumTitle = t.Album.Title,
                        ArtistName = t.Album.Artist.Name,
                        TrackId = t.TrackId,
                        TrackName = t.Name,
                        IsFavorite = t.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == CurrentUserId && up.Playlist.Name == Constants.Favourite)).Any()
                    }).ToList()
                }).FirstOrDefault();
            }
        }

        private async Task<string> GetUserId()
        {
            var user = (await authenticationState).User;
            var userId = user.FindFirst(u => u.Type.Contains(ClaimTypes.NameIdentifier))?.Value;
            return userId;
        }

        protected async void FavoriteTrack(long trackId)
        {
            var track = Playlist.Tracks.FirstOrDefault(t => t.TrackId == trackId);
            InfoMessage = $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} added to playlist Favorites.";

            //Add track to the favourite playlist under user
            _userPlaylistService.AddToFavourite(trackId, CurrentUserId);
            await LoadPlaylist();
            await InvokeAsync(StateHasChanged);
        }

        protected async void UnfavoriteTrack(long trackId)
        {
            var track = Playlist.Tracks.FirstOrDefault(t => t.TrackId == trackId);
            InfoMessage = $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} removed from playlist Favorites.";

            //Remove track from favorite list under user
            _userPlaylistService.RemoveFromFavourite(trackId, CurrentUserId, PlaylistId);
            await LoadPlaylist();

            await InvokeAsync(StateHasChanged);
        }

        protected async void RemoveTrack(long trackId)
        {
            InfoMessage = $"Track removed from playlist Favorites.";
            _userPlaylistService.RemoveFromPlaylist(trackId, CurrentUserId, PlaylistId);
            await LoadPlaylist();
        }

        protected void CloseInfoMessage()
        {
            InfoMessage = "";
        }
    }
}
