# PlaylistsNET

Simple library for reading and writing playlist's files. Supported formats: m3u, pls, wpl, zpl.

## Examples

### Read
```c#
WplContent content = new WplContent();
WplPlaylist playlist = content.GetFromStream(stream);

// or
var parser = PlaylistParserFactory.GetPlaylistParser(".m3u");
IBasePlaylist playlist = parser.GetFromStream(stream);
List<string> paths = playlist.GetTracksPaths();

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

string text = content.ToText(playlist);
// or
string text = PlaylistToTextHelper.ToText(playlist);
/*
#EXTM3U
#EXTALB:New album
#EXTINF:175,Song Title
C:\Music\songs.mp3
*/
```
