﻿using Chinook.Models;

namespace Chinook.Services
{
    public interface IArtistService
    {
        Task<List<Artist>> GetArtists();
        Task<List<Track>> GetAlbumTracks(long artistId);
        Task<Artist> GetArtistById(long id);
    }
}
