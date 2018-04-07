using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlaylistsNET.Content;
using PlaylistsNET.Models;
using System;
using System.IO;

namespace PlaylistsNET.Tests
{
    [TestClass]
    public class WplTests
    {
        [TestMethod]
        public void Create_CreatePlaylistAndCompareWithFile_Equal()
        {
            WplContent content = new WplContent();
            WplPlaylist playlist = new WplPlaylist();
            playlist.Title = "Eurowizja";
            playlist.PlaylistEntries.Add(new WplPlaylistEntry()
            {
                AlbumArtist = null,
                AlbumTitle = "",
                Duration = TimeSpan.FromMilliseconds(185364),
                Path = @"D:\Muzyka\Eurowizja\Eurowizja 2014\Can-linn & Kasey Smith - Heartbeat(Irlandia).mp3",
                TrackArtist = "Can-linn & Kasey Smith",
                TrackTitle = "Heartbeat"
            });
            playlist.PlaylistEntries.Add(new WplPlaylistEntry()
            {
                AlbumArtist = "Elaiza",
                AlbumTitle = "Eurovision Song Contest 2014",
                Duration = TimeSpan.Zero,
                Path = @"D:\Muzyka\Eurowizja\Eurowizja 2014\Elaiza - Is It Right.mp3",
                TrackArtist = "Elaiza",
                TrackTitle = "Is It Right"
            });

            string created = content.ToText(playlist);
            string fromFile = Helpers.Read("Playlist2.wpl");
            Assert.AreEqual(created, fromFile);
        }

        [TestMethod]
        public void GetFromStream_ReadPlaylistAndCompareWithObject_Equal()
        {
            WplContent content = new WplContent();
            WplPlaylist playlist = new WplPlaylist();
            playlist.Title = "Eurowizja";
            playlist.PlaylistEntries.Add(new WplPlaylistEntry()
            {
                AlbumArtist = null,
                AlbumTitle = "",
                Duration = TimeSpan.FromMilliseconds(185364),
                Path = @"D:\Muzyka\Eurowizja\Eurowizja 2014\Can-linn & Kasey Smith - Heartbeat(Irlandia).mp3",
                TrackArtist = "Can-linn & Kasey Smith",
                TrackTitle = "Heartbeat"
            });
            playlist.PlaylistEntries.Add(new WplPlaylistEntry()
            {
                AlbumArtist = "Elaiza",
                AlbumTitle = "Eurovision Song Contest 2014",
                Path = @"D:\Muzyka\Eurowizja\Eurowizja 2014\Elaiza - Is It Right.mp3",
                TrackArtist = "Elaiza",
                TrackTitle = "Is It Right"
            });

            var stream = Helpers.ReadStream("Playlist.wpl");
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
            WplContent content = new WplContent();
            WplPlaylist playlist = new WplPlaylist();
            playlist.Title = "";
            var stream = Helpers.ReadStream("Empty.wpl");
            var file = content.GetFromStream(stream);
            Assert.AreEqual(playlist.Title, file.Title);
            Assert.AreEqual(playlist.PlaylistEntries.Count, file.PlaylistEntries.Count);
            stream.Dispose();
        }

        [TestMethod]
        public void GetFromStream_ReadSmartPlaylistAndCompareWithObject_Equal()
        {
            WplContent content = new WplContent();
            WplPlaylist playlist = new WplPlaylist();
            playlist.Title = "Smart list";
            var stream = Helpers.ReadStream("Smart.wpl");
            var file = content.GetFromStream(stream);
            Assert.AreEqual(playlist.Title, file.Title);
            Assert.AreEqual(playlist.PlaylistEntries.Count, file.PlaylistEntries.Count);
            stream.Dispose();
        }

        [TestMethod]
        public void Update_UpdateFileSaveAndCompareWithCorrect_Equal()
        {
            WplContent content = new WplContent();
            WplPlaylist playlist = new WplPlaylist();
            playlist.Title = "abc";
            playlist.PlaylistEntries.Add(new WplPlaylistEntry()
            {
                AlbumArtist = "Nieznany wykonawca",
                AlbumTitle = "Nieznany album",
                Path = @"D:\abc.wav",
                TrackArtist = "Nieznany",
                TrackTitle = "abc"
            });
            playlist.Author = "me";
            playlist.Generator = "PlaylistsNET";
            var stream = Helpers.ReadStream("2seq.wpl");
            string updatedContent = content.Update(playlist, stream);
            stream.Dispose();
            Helpers.Save("2seqoutputTest.wpl", updatedContent);
            string updated = Helpers.Read("2seqoutputTest.wpl");
            string expected = Helpers.Read("2seqoutput.wpl");
            Assert.AreEqual(updated, expected);
        }

        [TestMethod]
        public void Test_UnEscape()
        {
            WplContent content = new WplContent();
            WplPlaylist playlist = new WplPlaylist();
            playlist.Title = "Eurowizja";
            playlist.PlaylistEntries.Add(new WplPlaylistEntry()
            {
                AlbumArtist = "E<>laiza",
                AlbumTitle = "E<u>r'o&vision \"Song Contest 2014",
                Duration = TimeSpan.Zero,
                Path = @"D:\Muzyka\Eurowizja\Eurowizja 2014\Elaiza - Is It Right.mp3",
                TrackArtist = "Elaiza",
                TrackTitle = "Is It Right"
            });

            string created = content.ToText(playlist);
            string fromFile = Helpers.Read("playlist3b.wpl");
            Assert.AreEqual(created, fromFile);
        }
    }
}
