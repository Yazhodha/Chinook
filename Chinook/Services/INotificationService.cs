using Chinook.Models;

namespace Chinook.Services
{
    public interface INotificationService
    {
        public event Action<Playlist> NewPlaylistAdded;
        public void RaisNewPlaylistAdded(Playlist playlist);
    }

    public class NotificationService : INotificationService
    {
        public event Action<Playlist> NewPlaylistAdded;

        public void RaisNewPlaylistAdded(Playlist playlist) => NewPlaylistAdded?.Invoke(playlist);

    }
}
