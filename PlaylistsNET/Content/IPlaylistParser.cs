using PlaylistsNET.Models;
using System.IO;

namespace PlaylistsNET.Content
{
    public interface IPlaylistParser<out T> where T : IBasePlaylist
    {
        T GetFromStream(Stream stream);
    }
}
