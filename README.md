# PlaylistsNET

Simple library for reading and writing playlist's files. Supported formats: m3u, pls, wpl, zpl.

## Examples

### Read
```c#
WplContent content = new WplContent();
WplPlaylist playlist = content.GetFromStream(stream);
```
### Save
```c#
M3uContent content = new M3uContent();
M3uPlaylist playlist = new M3uPlaylist();
playlist.IsExtended = true;
playlist.PlaylistEntries.Add(new M3uPlaylistEntry()
{
    Album = "New album",
    AlbumArtist = "",
    Duration = System.TimeSpan.FromSeconds(175),
    Path = @"C:\Music\songs.mp3",
    Title = "Song Title"
});
string toSave = content.ToText(playlist);
/*
#EXTM3U
#EXTALB:New album
#EXTINF:175,Song Title
C:\Music\songs.mp3
*/
```
