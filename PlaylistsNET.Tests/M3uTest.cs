using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlaylistsNET.Content;
using PlaylistsNET.Models;
using System;
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
        public void Create_CreatePlaylistExtendedAndCompareWithFile_Equal()
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
            var entry = new M3uPlaylistEntry()
            {
                Album = null,
                AlbumArtist = null,
                Duration = TimeSpan.FromSeconds(-1),
                Path = @"D:\Muzyka\Andrea Bocelli\01 Con Te Partiro.mp3",
                Title = "Andrea Bocelli - Con Te Partiro",                
            };
            entry.Properties.Add("EXTVLCOPT", "network-caching=1000");
            playlist.PlaylistEntries.Add(entry);
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

			using (var stream = Helpers.ReadStream("PlaylistNotExt.m3u"))
			{
				var file = content.GetFromStream(stream);

				Assert.AreEqual(playlist.IsExtended, file.IsExtended);
				Assert.AreEqual(playlist.PlaylistEntries.Count, file.PlaylistEntries.Count);

				Assert.AreEqual(playlist.PlaylistEntries[0].Path, file.PlaylistEntries[0].Path);
				Assert.AreEqual(playlist.PlaylistEntries[0].Title, file.PlaylistEntries[0].Title);

				Assert.AreEqual(playlist.PlaylistEntries[1].Path, file.PlaylistEntries[1].Path);
				Assert.AreNotEqual(playlist.PlaylistEntries[1].Title, file.PlaylistEntries[1].Title);
                Assert.IsNull(playlist.PlaylistEntries[1].Title);
                Assert.AreEqual("", file.PlaylistEntries[1].Title);

                Assert.AreEqual(playlist.PlaylistEntries[2].Path, file.PlaylistEntries[2].Path);
				Assert.AreNotEqual(playlist.PlaylistEntries[2].Title, file.PlaylistEntries[2].Title);
			}
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
            var entry = new M3uPlaylistEntry()
            {
                Album = null,
                AlbumArtist = null,
                Duration = TimeSpan.FromSeconds(-1),
                Path = @"D:\Muzyka\Andrea Bocelli\01 Con Te Partiro.mp3",
                Title = "Andrea Bocelli - Con Te Partiro",
            };
            entry.Properties.Add("EXTVLCOPT", "network-caching=1000");
            playlist.PlaylistEntries.Add(entry);
            playlist.PlaylistEntries.Add(new M3uPlaylistEntry()
            {
                Album = "AndreaBocelli",
                AlbumArtist = "Andrea Bocelli",
                Duration = TimeSpan.FromSeconds(-1),
                Path = @"D:\Muzyka\Andrea Bocelli\04 E Chiove.mp3",
                Title = "Andrea Bocelli - E Chiove",
            });

			using (var stream = Helpers.ReadStream("PlaylistExt.m3u"))
			{
				var file = content.GetFromStream(stream);

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
                Assert.IsNull(playlist.PlaylistEntries[1].Album);
                Assert.AreEqual("", file.PlaylistEntries[1].Album);

                Assert.AreEqual(3, playlist.PlaylistEntries[1].Properties.Count());
                Assert.AreEqual(1, file.PlaylistEntries[1].Properties.Count());
				Assert.AreEqual(playlist.PlaylistEntries[1].Properties.Last().Key, file.PlaylistEntries[1].Properties.Last().Key);
				Assert.AreEqual(playlist.PlaylistEntries[1].Properties.Last().Value, file.PlaylistEntries[1].Properties.Last().Value);

				Assert.AreEqual(playlist.PlaylistEntries[2].Path, file.PlaylistEntries[2].Path);
				Assert.AreEqual(playlist.PlaylistEntries[2].Title, file.PlaylistEntries[2].Title);
                Assert.AreEqual(playlist.PlaylistEntries[2].Album, file.PlaylistEntries[2].Album);
                Assert.AreEqual(playlist.PlaylistEntries[2].AlbumArtist, file.PlaylistEntries[2].AlbumArtist);
            }
        }
	}
}
