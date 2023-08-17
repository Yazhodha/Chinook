using Chinook.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Chinook.Shared
{
    public class NavMenuBase : ComponentBase
    {
        protected bool collapseNavMenu = true;
        protected List<Models.Playlist> playlists { get; set; }
        [Inject] IUserPlaylistService _userPlaylistService { get; set; }
        [Inject] INotificationService _notificationService { get; set; }
        [Inject] NavigationManager _navigationManager { get; set; }
        [CascadingParameter] private Task<AuthenticationState> authenticationState { get; set; }

        protected string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        protected void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }


        protected void onNavigationClick(long playlistId)
        {
            _navigationManager.NavigateTo($"playlist/{playlistId}", true);
        }

        protected async Task<string> GetUserId()
        {
            var user = (await authenticationState).User;
            var userId = user.FindFirst(u => u.Type.Contains(ClaimTypes.NameIdentifier))?.Value;
            return userId;
        }

        protected override async Task OnInitializedAsync()
        {
            _notificationService.NewPlaylistAdded += OnNewPlaylistAdded;

            playlists = await _userPlaylistService.GetPlaylistsByUserId(await GetUserId());
        }

        private void OnNewPlaylistAdded(Models.Playlist newPlaylist)
        {
            playlists.Add(newPlaylist);
            StateHasChanged();
        }
    }
}
