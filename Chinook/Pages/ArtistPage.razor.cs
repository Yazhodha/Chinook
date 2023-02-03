using Chinook.ClientModels;
using Chinook.Models;
using Chinook.Services;
using Chinook.Shared.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Chinook.Pages
{
    public class ArtistPageBase : ComponentBase
    {
        [Parameter] public long ArtistId { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationState { get; set; }
        [Inject] IArtistService _artistService { get; set; }
        [Inject] IUserPlaylistService _userPlaylistService { get; set; }
        protected Modal PlaylistDialog { get; set; }

        protected Artist Artist;
        protected List<PlaylistTrack> Tracks;
        protected DbContext DbContext;
        protected PlaylistTrack SelectedTrack;
        protected string InfoMessage;
        protected string CurrentUserId;
        public string ExistingPlaylistName { get; set; }
        public List<ClientModels.Playlist> Playlists { get; set; }
        public string NewPlaylistName { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await InvokeAsync(StateHasChanged);
            CurrentUserId = await GetUserId();

            Artist = await _artistService.GetArtistById(ArtistId);
            await LoadTrackList();

            //Loading existing playlists on modal
            Playlists = (await _userPlaylistService.GetPlaylistsByUserId(CurrentUserId))
                .Select(p => new ClientModels.Playlist()
                {
                    Name = p.Name,
                }).ToList();
        }

        private async Task LoadTrackList()
        {
            Tracks = (await _artistService.GetAlbumTracks(ArtistId))
                .Select(t => new PlaylistTrack()
                {
                    AlbumTitle = (t.Album == null ? "-" : t.Album.Title),
                    TrackId = t.TrackId,
                    TrackName = t.Name,
                    IsFavorite = t.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == CurrentUserId && up.Playlist.Name == "Favorites")).Any()
                })
                .ToList();
        }

        protected async Task<string> GetUserId()
        {
            var user = (await authenticationState).User;
            var userId = user.FindFirst(u => u.Type.Contains(ClaimTypes.NameIdentifier))?.Value;
            return userId;
        }

        protected async void FavoriteTrack(long trackId)
        {
            var track = Tracks.FirstOrDefault(t => t.TrackId == trackId);
            InfoMessage = $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} added to playlist Favorites.";

            //Add track to the favourite playlist
            _userPlaylistService.AddToFavourite(trackId, CurrentUserId);
            await LoadTrackList();
        }

        protected void UnfavoriteTrack(long trackId)
        {
            var track = Tracks.FirstOrDefault(t => t.TrackId == trackId);
            InfoMessage = $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} removed from playlist Favorites.";
        }

        protected void OpenPlaylistDialog(long trackId)
        {
            CloseInfoMessage();
            SelectedTrack = Tracks.FirstOrDefault(t => t.TrackId == trackId);
            PlaylistDialog.Open();
        }

        protected async void AddTrackToPlaylist()
        {
            CloseInfoMessage();
            InfoMessage = $"Track {Artist.Name} - {SelectedTrack.AlbumTitle} - {SelectedTrack.TrackName} added to playlist {{playlist name}}.";

            _userPlaylistService.AddPlaylist(NewPlaylistName ?? ExistingPlaylistName, CurrentUserId, SelectedTrack.TrackId);

            PlaylistDialog.Close();
        }

        protected void CloseInfoMessage()
        {
            InfoMessage = "";
        }
    }
}
