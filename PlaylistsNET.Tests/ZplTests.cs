using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlaylistsNET.Content;
using PlaylistsNET.Models;
using System;

namespace PlaylistsNET.Tests
{
    [TestClass]
    public class ZplTests
    {
        [TestMethod]
        public void Create_CreatePlaylistAndCompareWithFile_Equal()
        {
            ZplContent content = new ZplContent();
            ZplPlaylist playlist = new ZplPlaylist();
            playlist.Title = "Eurowizja";
            playlist.PlaylistEntries.Add(new ZplPlaylistEntry()
            {
                AlbumArtist = null,
                AlbumTitle = "",
                Duration = TimeSpan.FromMilliseconds(185364),
                Path = @"D:\Muzyka\Eurowizja\Eurowizja 2014\Can-linn & Kasey Smith - Heartbeat(Irlandia).mp3",
                TrackArtist = "Can-linn & Kasey Smith",
                TrackTitle = "Heartbeat"
            });
            playlist.PlaylistEntries.Add(new ZplPlaylistEntry()
            {
                AlbumArtist = "Elaiza",
                AlbumTitle = "Eurovision Song Contest 2014",
                Duration = TimeSpan.Zero,
                Path = @"D:\Muzyka\Eurowizja\Eurowizja 2014\Elaiza - Is It Right.mp3",
                TrackArtist = "Elaiza",
                TrackTitle = "Is It Right"
            });

            string created = content.ToText(playlist);
            string fromFile = Helpers.Read("Playlist2.zpl");
            Assert.AreEqual(created, fromFile);
        }

        [TestMethod]
        public void GetFromStream_ReadPlaylistAndCompareWithObject_Equal()
        {
            ZplContent content = new ZplContent();
            ZplPlaylist playlist = new ZplPlaylist();
            playlist.Title = "Eurowizja";
            playlist.PlaylistEntries.Add(new ZplPlaylistEntry()
            {
                AlbumArtist = null,
                AlbumTitle = "",
                Duration = TimeSpan.FromMilliseconds(185364),
                Path = @"D:\Muzyka\Eurowizja\Eurowizja 2014\Can-linn & Kasey Smith - Heartbeat(Irlandia).mp3",
                TrackArtist = "Can-linn & Kasey Smith",
                TrackTitle = "Heartbeat"
            });
            playlist.PlaylistEntries.Add(new ZplPlaylistEntry()
            {
                AlbumArtist = "Elaiza",
                AlbumTitle = "Eurovision Song Contest 2014",
                Path = @"D:\Muzyka\Eurowizja\Eurowizja 2014\Elaiza - Is It Right.mp3",
                TrackArtist = "Elaiza",
                TrackTitle = "Is It Right"
            });

            var stream = Helpers.ReadStream("Playlist.zpl");
            var file = content.GetFromStream(stream);
            stream.Dispose();
            Assert.AreEqual(playlist.PlaylistEntries.Count, file.PlaylistEntries.Count);
            Assert.AreEqual(playlist.Title, file.Title);

            Assert.AreEqual(playlist.PlaylistEntries[0].AlbumArtist, file.PlaylistEntries[0].AlbumArtist);
            Assert.AreEqual(playlist.PlaylistEntries[1].AlbumArtist, file.PlaylistEntries[1].AlbumArtist);

            Assert.AreEqual(String.IsNullOrEmpty(playlist.PlaylistEntries[0].AlbumTitle), String.IsNullOrEmpty(file.PlaylistEntries[0].AlbumTitle));
            Assert.AreEqual(playlist.PlaylistEntries[1].AlbumTitle, file.PlaylistEntries[1].AlbumTitle);

            Assert.AreEqual(playlist.PlaylistEntries[0].TrackArtist, file.PlaylistEntries[0].TrackArtist);
            Assert.AreEqual(playlist.PlaylistEntries[1].TrackArtist, file.PlaylistEntries[1].TrackArtist);

            Assert.AreEqual(playlist.PlaylistEntries[0].TrackTitle, file.PlaylistEntries[0].TrackTitle);
            Assert.AreEqual(playlist.PlaylistEntries[1].TrackTitle, file.PlaylistEntries[1].TrackTitle);

            Assert.AreEqual(playlist.PlaylistEntries[0].Path, file.PlaylistEntries[0].Path);
            Assert.AreEqual(playlist.PlaylistEntries[1].Path, file.PlaylistEntries[1].Path);
            stream.Dispose();
        }

        [TestMethod]
        public void GetFromStream_ReadEmptyPlaylistAndCompareWithObject_Equal()
        {
            ZplContent content = new ZplContent();
            ZplPlaylist playlist = new ZplPlaylist();
            playlist.Title = "";
            var stream = Helpers.ReadStream("Empty.zpl");
            var file = content.GetFromStream(stream);
            Assert.AreEqual(playlist.Title, file.Title);
            Assert.AreEqual(playlist.PlaylistEntries.Count, file.PlaylistEntries.Count);
            stream.Dispose();
        }
    }
}
