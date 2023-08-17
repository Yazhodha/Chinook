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
        public string ExistingPlaylistName { get; set; } = Constants.Favourite;
        public List<ClientModels.Playlist> Playlists { get; set; }
        public string? NewPlaylistName { get; set; } = null;

        protected override async Task OnInitializedAsync()
        {
            CurrentUserId = await GetUserId();
            await LoadTrackList();

            Artist = await _artistService.GetArtistById(ArtistId);

            //Loading existing playlists on modal
            Playlists = (await _userPlaylistService.GetPlaylistsByUserId(CurrentUserId))
                .Select(p => new ClientModels.Playlist()
                {
                    Name = p.Name,
                }).ToList();
        }

        private async Task LoadTrackList()
        {
            var tracks = await _artistService.GetAlbumTracks(ArtistId);
            Tracks = tracks
                .Select(t => new PlaylistTrack()
                {
                    AlbumTitle = (t.Album == null ? "-" : t.Album.Title),
                    TrackId = t.TrackId,
                    TrackName = t.Name,
                    IsFavorite = t.Playlists.Where(p => p.UserPlaylists.Any(up => up.UserId == CurrentUserId && up.Playlist.Name == Constants.Favourite)).Any()
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
            try
            {
                var track = Tracks.FirstOrDefault(t => t.TrackId == trackId);
                InfoMessage = $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} added to playlist {Constants.Favourite}.";

                // Add track to the favourite playlist
                _userPlaylistService.AddToFavourite(trackId, CurrentUserId);

                // Update the Tracks list to reflect the favourited track
                var updatedTrack = Tracks.FirstOrDefault(t => t.TrackId == trackId);
                if (updatedTrack != null)
                {
                    updatedTrack.IsFavorite = true;
                }

                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                // Handle exception and show error message
            }
        }

        protected async void UnfavoriteTrack(long trackId)
        {
            var track = Tracks.FirstOrDefault(t => t.TrackId == trackId);
            InfoMessage = $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} removed from playlist {Constants.Favourite}.";

            _userPlaylistService.RemoveFromFavourite(trackId, CurrentUserId);

            // Update the Tracks list to reflect the unfavourited track
            var updatedTrack = Tracks.FirstOrDefault(t => t.TrackId == trackId);
            if (updatedTrack != null)
            {
                updatedTrack.IsFavorite = false;
            }

            await InvokeAsync(StateHasChanged);
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
            InfoMessage = $"Track {Artist.Name} - {SelectedTrack.AlbumTitle} - {SelectedTrack.TrackName} added to playlist {NewPlaylistName ?? ExistingPlaylistName}.";

            _userPlaylistService.AddPlaylist(NewPlaylistName ?? ExistingPlaylistName, CurrentUserId, SelectedTrack.TrackId);

            PlaylistDialog.Close();
        }

        protected void CloseInfoMessage()
        {
            InfoMessage = "";
        }
    }
}
