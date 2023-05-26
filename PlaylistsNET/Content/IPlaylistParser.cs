using PlaylistsNET.Models;
using System.IO;
using System.Text;

namespace PlaylistsNET.Content
{
    public interface IPlaylistParser<out T> where T : IBasePlaylist
    {
        T GetFromStream(Stream stream);
        T GetFromStream(Stream stream, Encoding encoding);
        T GetFromString(string playlistString);
    }
}
