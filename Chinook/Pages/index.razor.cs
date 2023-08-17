using Chinook.Models;
using Chinook.Services;
using Microsoft.AspNetCore.Components;

namespace Chinook.Pages
{
    public class IndexBase : ComponentBase
    {
        protected List<Artist> Artists;
        protected List<Artist> FilteredArtists;
        protected string SearchQuery = "";
        [Inject] IArtistService ArtistService { get; set; }
        [Inject] IAlbumService AlbumService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await InvokeAsync(StateHasChanged);
            await LoadArtists();
        }

        public async Task LoadArtists()
        {
            Artists = await GetArtists();
            FilterArtists();
        }

        public async Task<List<Artist>> GetArtists()
        {
            return await ArtistService.GetArtists();
        }

        public async Task<List<Album>> GetAlbumsForArtist(int artistId)
        {
            return await AlbumService.GetAlbumsForArtist(artistId);
        }

        protected void SearchArtists()
        {
            FilterArtists();
        }

        private void FilterArtists()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                FilteredArtists = Artists;
            }
            else
            {
                FilteredArtists = Artists
                    .Where(artist => artist.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
        }

    }
}
