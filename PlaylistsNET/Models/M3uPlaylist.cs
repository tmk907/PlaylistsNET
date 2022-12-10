using System.Collections.Generic;

namespace PlaylistsNET.Models
{
    public class M3uPlaylist : BasePlaylist<M3uPlaylistEntry>
    {
        public M3uPlaylist()
        {
            Comments = new List<string>();
        }

        public bool IsExtended { get; set; }
        public List<string> Comments { get; set; }
    }
}
