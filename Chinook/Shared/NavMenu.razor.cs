using Chinook.Models;
using Chinook.Services;
using Microsoft.AspNetCore.Components;

namespace Chinook.Shared
{
    public class NavMenuBase : ComponentBase
    {
        protected bool collapseNavMenu = true;
        protected List<Playlist> playlists { get; set; }
        [Inject] IUserPlaylistService _userPlaylistService { get; set; }

        protected string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        protected void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

        protected override async Task OnInitializedAsync()
        {
            await InvokeAsync(StateHasChanged);

            playlists = await _userPlaylistService.GetAllUserPlaylist();
        }

    }
}
