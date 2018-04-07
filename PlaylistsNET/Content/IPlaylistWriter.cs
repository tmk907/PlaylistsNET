using PlaylistsNET.Models;

namespace PlaylistsNET.Content
{
    public interface IPlaylistWriter<T> where T : IBasePlaylist
    {
        string ToText(T playlist);
    }
}
