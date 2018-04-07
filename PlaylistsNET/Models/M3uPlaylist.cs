namespace PlaylistsNET.Models
{
    public class M3uPlaylist : BasePlaylist<M3uPlaylistEntry>
    {
        public bool IsExtended { get; set; }
    }
}
