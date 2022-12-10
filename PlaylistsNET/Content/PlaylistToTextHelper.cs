using PlaylistsNET.Models;

namespace PlaylistsNET.Content
{
    public class PlaylistToTextHelper
    {
        public static string ToText(IBasePlaylist playlist)
        {
            string text = "";

            switch (playlist)
            {
                case M3uPlaylist m3u:
                    var m3uWriter = new M3uContent();
                    text = m3uWriter.ToText(m3u);
                    break;
                case PlsPlaylist pls:
                    var plsWriter = new PlsContent();
                    text = plsWriter.ToText(pls);
                    break;
                case WplPlaylist wpl:
                    var wplWriter = new WplContent();
                    text = wplWriter.ToText(wpl);
                    break;
                case ZplPlaylist zpl:
                    var zplWriter = new ZplContent();
                    text = zplWriter.ToText(zpl);
                    break;
                case HlsMasterPlaylist hls:
                    var hlsMasterWriter = new HlsMasterContent();
                    text = hlsMasterWriter.ToText(hls);
                    break;
                case HlsMediaPlaylist hls:
                    var hlsMediaWriter = new HlsMediaContent();
                    text = hlsMediaWriter.ToText(hls);
                    break;
                default:
                    break;
            }

            return text;
        }
    }
}
