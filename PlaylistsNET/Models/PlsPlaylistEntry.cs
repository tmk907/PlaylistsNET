using System;

namespace PlaylistsNET.Models
{
    public class PlsPlaylistEntry : BasePlaylistEntry
    {
        public string Title { get; set; }
        public TimeSpan Length { get; set; }
        public int Nr { get; set; }
    }
}
