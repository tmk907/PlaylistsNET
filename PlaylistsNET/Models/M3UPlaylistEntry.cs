using System;
using System.Collections.Generic;
using System.Linq;

namespace PlaylistsNET.Models
{
    public class M3uPlaylistEntry : BasePlaylistEntry
    {
        public M3uPlaylistEntry()
        {
            CustomProperties = Enumerable.Empty<KeyValuePair<string, string>>();
        }

        public TimeSpan Duration { get; set; }
        public string Title { get; set; }
        public string Album { get; set; }
        public string AlbumArtist { get; set; }
        public IEnumerable<KeyValuePair<string, string>> CustomProperties { get; set; }
    }
}
