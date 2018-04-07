using System;

namespace PlaylistsNET.Models
{
    public class ZplPlaylist : BasePlaylist<ZplPlaylistEntry>
    {
        public string Author { get; set; }
        public string Generator { get; set; }
        public string Guid { get; set; }
        public int ItemCount { get; set; }
        public string Title { get; set; }
        public TimeSpan TotalDuration { get; set; }
    }
}
