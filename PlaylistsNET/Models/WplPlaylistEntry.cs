using System;

namespace PlaylistsNET.Models
{
    public class WplPlaylistEntry : BasePlaylistEntry
    {
        public string AlbumTitle { get; set; }
        public string AlbumArtist { get; set; }
        public TimeSpan Duration { get; set; }
        public string TrackTitle { get; set; }
        public string TrackArtist { get; set; }
    }
}
