using Chinook.Models;
using Chinook.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Pages
{
    public class IndexBase : ComponentBase
    {
        protected List<Artist> Artists;
        [Inject] IArtistService ArtistService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await InvokeAsync(StateHasChanged);
            Artists = await GetArtists();
        }

        public async Task<List<Artist>> GetArtists()
        {
            return await ArtistService.GetArtists();
        }

        //public async Task<List<Album>> GetAlbumsForArtist(int artistId)
        //{
        //    //var dbContext = await DbFactory.CreateDbContextAsync();
        //    //return dbContext.Albums.Where(a => a.ArtistId == artistId).ToList();
        //}
    }
}
