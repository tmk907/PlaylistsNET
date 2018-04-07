using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlaylistsNET.Content;
using PlaylistsNET.Models;
using System;

namespace PlaylistsNET.Tests
{
    [TestClass]
    public class PlsTests
    {
        [TestMethod]
        public void Create_CreatePlaylistAndCompareWithFile_Equal()
        {
            PlsContent content = new PlsContent();
            PlsPlaylist playlist = new PlsPlaylist();
            playlist.Version = 2;
            playlist.PlaylistEntries.Add(new PlsPlaylistEntry()
            {
                Length = TimeSpan.Zero,
                Nr = 1,
                Path = "http://stream3.polskieradio.pl:8902/",
                Title = null,
            });
            playlist.PlaylistEntries.Add(new PlsPlaylistEntry()
            {
                Length = TimeSpan.FromSeconds(-1),
                Nr = 1,
                Path = "http://stream.polskastacja.pl/ps43_mp3?player_group=PS_EXT_MP3",
                Title = "Server1-> >>> P O L S K A S T A C J A <<<- Ballady Rockowe",
            });
            playlist.PlaylistEntries.Add(new PlsPlaylistEntry()
            {
                Length = TimeSpan.FromSeconds(5720),
                Nr = 1,
                Path = "/home/uzytkownik/muzyka-1.mp3",
                Title = "Myslovitz - Sprzedawcy Marzeń",
            });

            string created = content.ToText(playlist);
            string fromFile = Helpers.Read("Playlist.pls");
            Assert.AreEqual(created, fromFile);
        }

        [TestMethod]
        public void GetFromStream_ReadPlaylistAndCompareWithObject_Equal()
        {
            PlsContent content = new PlsContent();
            PlsPlaylist playlist = new PlsPlaylist();
            playlist.Version = 2;
            playlist.PlaylistEntries.Add(new PlsPlaylistEntry()
            {
                Length = TimeSpan.Zero,
                Nr = 1,
                Path = "http://stream3.polskieradio.pl:8902/",
                Title = null,
            });
            playlist.PlaylistEntries.Add(new PlsPlaylistEntry()
            {
                Length = TimeSpan.FromSeconds(-1),
                Nr = 2,
                Path = "http://stream.polskastacja.pl/ps43_mp3?player_group=PS_EXT_MP3",
                Title = "Server1-> >>> P O L S K A S T A C J A <<<- Ballady Rockowe",
            });
            playlist.PlaylistEntries.Add(new PlsPlaylistEntry()
            {
                Length = TimeSpan.FromSeconds(5720),
                Nr = 3,
                Path = "/home/uzytkownik/muzyka-1.mp3",
                Title = "Myslovitz - Sprzedawcy Marzeń",
            });
            playlist.PlaylistEntries.Add(new PlsPlaylistEntry()
            {
                Length = TimeSpan.Zero,
                Nr = 4,
                Path = "Weird Al - This Is The Life.mp3",
                Title = "Weird Al Yankovic - This is the Life",
            });
            
            var stream = Helpers.ReadStream("Playlist2.pls");
            var file = content.GetFromStream(stream);
            stream.Dispose();
            Assert.AreEqual(playlist.PlaylistEntries.Count, file.PlaylistEntries.Count);
            Assert.AreEqual(playlist.NumberOfEntries, file.NumberOfEntries);
            Assert.AreEqual(playlist.Version, file.Version);

            Assert.AreEqual(playlist.PlaylistEntries[0].Path, file.PlaylistEntries[0].Path);
            Assert.AreEqual(playlist.PlaylistEntries[1].Path, file.PlaylistEntries[1].Path);
            Assert.AreEqual(playlist.PlaylistEntries[2].Path, file.PlaylistEntries[2].Path);
            Assert.AreEqual(playlist.PlaylistEntries[3].Path, file.PlaylistEntries[3].Path);

            Assert.AreEqual(playlist.PlaylistEntries[0].Title, file.PlaylistEntries[0].Title);
            Assert.AreEqual(playlist.PlaylistEntries[1].Title, file.PlaylistEntries[1].Title);
            Assert.AreEqual(playlist.PlaylistEntries[2].Title, file.PlaylistEntries[2].Title);
            Assert.AreEqual(playlist.PlaylistEntries[3].Title, file.PlaylistEntries[3].Title);

            Assert.AreEqual(playlist.PlaylistEntries[0].Length, file.PlaylistEntries[0].Length);
            Assert.AreEqual(playlist.PlaylistEntries[1].Length, file.PlaylistEntries[1].Length);
            Assert.AreEqual(playlist.PlaylistEntries[2].Length, file.PlaylistEntries[2].Length);
            Assert.AreEqual(playlist.PlaylistEntries[3].Length, file.PlaylistEntries[3].Length);

            Assert.AreEqual(playlist.PlaylistEntries[0].Nr, file.PlaylistEntries[0].Nr);
            Assert.AreEqual(playlist.PlaylistEntries[1].Nr, file.PlaylistEntries[1].Nr);
            Assert.AreEqual(playlist.PlaylistEntries[2].Nr, file.PlaylistEntries[2].Nr);
            Assert.AreEqual(playlist.PlaylistEntries[3].Nr, file.PlaylistEntries[3].Nr);
            stream.Dispose();
        }
    }
}
