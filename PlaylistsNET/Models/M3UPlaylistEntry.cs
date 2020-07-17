using System;
using System.Collections.Generic;

namespace PlaylistsNET.Models
{
    public class M3uPlaylistEntry : BasePlaylistEntry
    {
        public M3uPlaylistEntry()
        {
            CustomProperties = new Dictionary<string, string>();
            Comments = new List<string>();
        }

        public TimeSpan Duration { get; set; }
        public string Title { get; set; }
        public string Album { get; set; }
        public string AlbumArtist { get; set; }
        public Dictionary<string, string> CustomProperties { get; set; }
        public List<string> Comments { get; set; }
    }
}