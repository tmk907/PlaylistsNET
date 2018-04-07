namespace PlaylistsNET.Models
{
    public class PlsPlaylist : BasePlaylist<PlsPlaylistEntry>
    {
        public PlsPlaylist()
        {
            Version = 2;
        }
        public int Version { get; set; }
        public int NumberOfEntries { get { return PlaylistEntries.Count; } }
    }
}
