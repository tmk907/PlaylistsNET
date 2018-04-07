using PlaylistsNET.Models;

namespace PlaylistsNET.Content
{
    public class PlaylistParserFactory
    {
        public static IPlaylistParser<IBasePlaylist> GetPlaylistParser(string fileType)
        {
            IPlaylistParser<IBasePlaylist> playlistParser;
            fileType = fileType.ToLower();
            switch (fileType)
            {
                case ".m3u":
                    playlistParser = new M3uContent();
                    break;
                case ".m3u8":
                    playlistParser = new M3u8Content();
                    break;
                case ".pls":
                    playlistParser = new PlsContent();
                    break;
                case ".wpl":
                    playlistParser = new WplContent();
                    break;
                case ".zpl":
                    playlistParser = new ZplContent();
                    break;
                default:
                    playlistParser = null;
                    break;
            }
            return playlistParser;
        }
    }
}
