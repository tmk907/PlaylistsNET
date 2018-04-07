using PlaylistsNET.Models;

namespace PlaylistsNET.Content
{
    public class PlaylistToTextHelper
    {
        public string ToString(IBasePlaylist playlist)
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
                default:
                    break;
            }

            return text;
        }
    }
}
