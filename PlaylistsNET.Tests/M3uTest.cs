using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlaylistsNET.Content;
using PlaylistsNET.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlaylistsNET.Tests
{
    [TestClass]
    public class M3uTest
    {
        [TestMethod]
        public void Create_CreatePlaylistAndCompareWithFile_Equal()
        {
            M3uContent content = new M3uContent();
            M3uPlaylist playlist = new M3uPlaylist();
            playlist.IsExtended = false;
            playlist.PlaylistEntries.Add(new M3uPlaylistEntry()
            {
                Album = "",
                AlbumArtist = "",
                Duration = TimeSpan.Zero,
                Path = @"D:\Muzyka\Andrea Bocelli\04 Chiara.mp3",
                Title = "",
            });
            playlist.PlaylistEntries.Add(new M3uPlaylistEntry()
            {
                Album = null,
                AlbumArtist = null,
                Duration = TimeSpan.Zero,
                Path = @"D:\Muzyka\Andrea Bocelli\01 Con Te Partiro.mp3",
                Title = null,
            });
            playlist.PlaylistEntries.Add(new M3uPlaylistEntry()
            {
                Album = "Unknown",
                AlbumArtist = "Andrea Bocelli",
                Duration = TimeSpan.Zero,
                Path = @"D:\Muzyka\Andrea Bocelli\04 E Chiove.mp3",
                Title = "E Chiove",
            });
            string created = content.ToText(playlist);
            string fromFile = Helpers.Read("PlaylistNotExt.m3u");
            Assert.AreEqual(created, fromFile);
        }

        [TestMethod]
        public void Create_CreatePlaylistExtentedAndCompareWithFile_Equal()
        {
            M3uContent content = new M3uContent();
            M3uPlaylist playlist = new M3uPlaylist();
            playlist.IsExtended = true;
            playlist.PlaylistEntries.Add(new M3uPlaylistEntry()
            {
                Album = "",
                AlbumArtist = "",
                Duration = TimeSpan.FromSeconds(254),
                Path = @"D:\Muzyka\Andrea Bocelli\04 Chiara.mp3",
                Title = "Andrea Bocelli - Chiara",
            });
            playlist.PlaylistEntries.Add(new M3uPlaylistEntry()
            {
                Album = null,
                AlbumArtist = null,
                Duration = TimeSpan.FromSeconds(-1),
                Path = @"D:\Muzyka\Andrea Bocelli\01 Con Te Partiro.mp3",
                Title = "Andrea Bocelli - Con Te Partiro",
                CustomProperties = new Dictionary<string, string>
                {
                    ["EXTVLCOPT"] = "network-caching=1000"
                },
            });
            playlist.PlaylistEntries.Add(new M3uPlaylistEntry()
            {
                Album = "AndreaBocelli",
                AlbumArtist = "Andrea Bocelli",
                Duration = TimeSpan.FromSeconds(-1),
                Path = @"D:\Muzyka\Andrea Bocelli\04 E Chiove.mp3",
                Title = "Andrea Bocelli - E Chiove",
            });
            string created = content.ToText(playlist);
            string fromFile = Helpers.Read("PlaylistExt.m3u");
            Assert.AreEqual(created, fromFile);
        }

        [TestMethod]
        public void GetFromStream_ReadPlaylistNotExtendedAndCompareWithObject_Equal()
        {
            M3uContent content = new M3uContent();
            M3uPlaylist playlist = new M3uPlaylist();
            playlist.IsExtended = false;
            playlist.PlaylistEntries.Add(new M3uPlaylistEntry()
            {
                Album = "",
                AlbumArtist = "",
                Duration = TimeSpan.Zero,
                Path = @"D:\Muzyka\Andrea Bocelli\04 Chiara.mp3",
                Title = "",
            });
            playlist.PlaylistEntries.Add(new M3uPlaylistEntry()
            {
                Album = null,
                AlbumArtist = null,
                Duration = TimeSpan.Zero,
                Path = @"D:\Muzyka\Andrea Bocelli\01 Con Te Partiro.mp3",
                Title = null,
            });
            playlist.PlaylistEntries.Add(new M3uPlaylistEntry()
            {
                Album = "Unknown",
                AlbumArtist = "Andrea Bocelli",
                Duration = TimeSpan.Zero,
                Path = @"D:\Muzyka\Andrea Bocelli\04 E Chiove.mp3",
                Title = "E Chiove",
            });
            var stream = Helpers.ReadStream("PlaylistNotExt.m3u");
            var file = content.GetFromStream(stream);
            stream.Dispose();
            Assert.AreEqual(playlist.IsExtended, file.IsExtended);
            Assert.AreEqual(playlist.PlaylistEntries.Count, file.PlaylistEntries.Count);
            Assert.AreEqual(playlist.PlaylistEntries[0].Path, file.PlaylistEntries[0].Path);
            Assert.AreEqual(playlist.PlaylistEntries[0].Title, file.PlaylistEntries[0].Title);
            Assert.AreEqual(playlist.PlaylistEntries[1].Path, file.PlaylistEntries[1].Path);
            Assert.AreEqual("", file.PlaylistEntries[1].Title);
            Assert.AreEqual(playlist.PlaylistEntries[2].Path, file.PlaylistEntries[2].Path);
            Assert.AreNotEqual(playlist.PlaylistEntries[2].Title, file.PlaylistEntries[2].Title);
            stream.Dispose();
        }

        [TestMethod]
        public void GetFromStream_ReadPlaylistExtendedAndCompareWithObject_Equal()
        {
            M3uContent content = new M3uContent();
            M3uPlaylist playlist = new M3uPlaylist();
            playlist.IsExtended = true;
            playlist.PlaylistEntries.Add(new M3uPlaylistEntry()
            {
                Album = "",
                AlbumArtist = "",
                Duration = TimeSpan.FromSeconds(254),
                Path = @"D:\Muzyka\Andrea Bocelli\04 Chiara.mp3",
                Title = "Andrea Bocelli - Chiara",
            });
            playlist.PlaylistEntries.Add(new M3uPlaylistEntry()
            {
                Album = null,
                AlbumArtist = null,
                Duration = TimeSpan.FromSeconds(-1),
                Path = @"D:\Muzyka\Andrea Bocelli\01 Con Te Partiro.mp3",
                Title = "Andrea Bocelli - Con Te Partiro",
                CustomProperties = new Dictionary<string, string>
                {
                    ["EXTVLCOPT"] = "network-caching=1000"
                },
            });
            playlist.PlaylistEntries.Add(new M3uPlaylistEntry()
            {
                Album = "AndreaBocelli",
                AlbumArtist = "Andrea Bocelli",
                Duration = TimeSpan.FromSeconds(-1),
                Path = @"D:\Muzyka\Andrea Bocelli\04 E Chiove.mp3",
                Title = "Andrea Bocelli - E Chiove",
            });
            var stream = Helpers.ReadStream("PlaylistExt.m3u");
            var file = content.GetFromStream(stream);
            stream.Dispose();
            Assert.AreEqual(playlist.IsExtended, file.IsExtended);
            Assert.AreEqual(playlist.PlaylistEntries.Count, file.PlaylistEntries.Count);
            Assert.AreEqual(playlist.PlaylistEntries[0].Path, file.PlaylistEntries[0].Path);
            Assert.AreEqual(playlist.PlaylistEntries[0].Title, file.PlaylistEntries[0].Title);
            Assert.AreEqual(playlist.PlaylistEntries[0].Album, file.PlaylistEntries[0].Album);
            Assert.AreEqual(playlist.PlaylistEntries[0].AlbumArtist, file.PlaylistEntries[0].AlbumArtist);
            Assert.AreEqual(playlist.PlaylistEntries[1].Path, file.PlaylistEntries[1].Path);
            Assert.AreEqual(playlist.PlaylistEntries[1].Title, file.PlaylistEntries[1].Title);
            Assert.AreNotEqual(playlist.PlaylistEntries[1].Album, file.PlaylistEntries[1].Album);
            Assert.AreNotEqual(playlist.PlaylistEntries[1].AlbumArtist, file.PlaylistEntries[1].AlbumArtist);
            Assert.AreEqual(playlist.PlaylistEntries[1].CustomProperties.Count(), file.PlaylistEntries[1].CustomProperties.Count());
            Assert.AreEqual(playlist.PlaylistEntries[1].CustomProperties.First().Key, file.PlaylistEntries[1].CustomProperties.First().Key);
            Assert.AreEqual(playlist.PlaylistEntries[1].CustomProperties.First().Value, file.PlaylistEntries[1].CustomProperties.First().Value);
            Assert.AreEqual(playlist.PlaylistEntries[2].Path, file.PlaylistEntries[2].Path);
            Assert.AreEqual(playlist.PlaylistEntries[2].Title, file.PlaylistEntries[2].Title);
            Assert.AreEqual(playlist.PlaylistEntries[2].Album, file.PlaylistEntries[2].Album);
            Assert.AreEqual(playlist.PlaylistEntries[2].AlbumArtist, file.PlaylistEntries[2].AlbumArtist);
        }

    }
}
