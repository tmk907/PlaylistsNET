# PlaylistsNET

Simple library for reading and writing playlist's files. Supported formats: m3u, pls, wpl, zpl.

## Install

[![Nuget](https://img.shields.io/nuget/v/PlaylistsNet)](https://img.shields.io/nuget/v/PlaylistsNet) [https://www.nuget.org/packages/PlaylistsNET](https://www.nuget.org/packages/PlaylistsNET)

## Examples

### Read
```c#
WplContent content = new WplContent();
WplPlaylist playlist = content.GetFromStream(stream);
// or
var parser = PlaylistParserFactory.GetPlaylistParser(".wpl");
IBasePlaylist playlist = parser.GetFromStream(stream);

List<string> paths = playlist.GetTracksPaths();

```
### Save
```c#
M3uPlaylist playlist = new M3uPlaylist();
playlist.IsExtended = true;
playlist.PlaylistEntries.Add(new M3uPlaylistEntry()
{
    Album = "New album",
    AlbumArtist = "",
    Duration = TimeSpan.FromSeconds(175),
    Path = @"C:\Music\song.mp3",
    Title = "Track Title"
});

M3uContent content = new M3uContent();
string text = content.ToText(playlist);
// or
string text = PlaylistToTextHelper.ToText(playlist);

/*
#EXTM3U
#EXTALB:New album
#EXTINF:175,Track Title
C:\Music\song.mp3
*/
```
