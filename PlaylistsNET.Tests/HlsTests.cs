using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlaylistsNET.Content;
using PlaylistsNET.Models;
using System;
using System.IO;

namespace PlaylistsNET.Tests
{
	[TestClass]
	public class HlsTest
	{
		[TestMethod]
		public void GetFromStream_ReadPlaylistHlsMediaAndCompareWithObject_Equal()
		{
			var content = new HlsMediaContent();

			using (var stream = Helpers.ReadStream("PlaylistExtHls.m3u"))
			{
				var file = content.GetFromStream(stream);

				Assert.AreEqual("NO", file.AllowCache, false);
				Assert.AreEqual(1, file.Version);
				Assert.AreEqual(10, file.TargetDuration);
				Assert.AreEqual(56, file.PlaylistEntries.Count);

				var entry = file.PlaylistEntries[0];
				Assert.AreEqual(@"METHOD=AES-128,URI=""key""", entry.Key);
				Assert.AreEqual(10, entry.Duration);
				Assert.AreEqual(147483, entry.MediaSequence);
				Assert.AreEqual("stream1_256k_1_061532839410_00147483_v3.aac", entry.Path);

				entry = file.PlaylistEntries[1];
				Assert.AreEqual(@"METHOD=AES-128,URI=""key""", entry.Key);
				Assert.AreEqual(10, entry.Duration);
				Assert.AreEqual(147484, entry.MediaSequence);
				Assert.AreEqual("stream1_256k_1_061532849162_00147484_v3.aac", entry.Path);
			}
		}

		[TestMethod]
		public void GetFromStream_ReadPlaylistHlsMasterAndCompareWithObject_Equal()
		{
			var content = new HlsMasterContent();

			using (var stream = Helpers.ReadStream("PlaylistExtHlsMaster.m3u"))
			{
				var file = content.GetFromStream(stream);

				Assert.AreEqual("NO", file.AllowCache, false);
				Assert.AreEqual(1, file.Version);
				Assert.AreEqual(4, file.PlaylistEntries.Count);

				var entry = file.PlaylistEntries[0];
				Assert.AreEqual(1, entry.ProgramId);
				Assert.AreEqual(281600, entry.Bandwidth);
				Assert.AreEqual("mp4a.40.2", entry.Codecs[0]);
				Assert.AreEqual("HLS_9506_256k_v3/9506_256k_large_v3.m3u8", entry.Path);
			}
		}

		[TestMethod]
		public void Throw_exception_when_create_hls_from_non_hls()
		{
			var content = new HlsMasterContent();

			var playlistString = "this is not a playlist";
			Assert.ThrowsException<FormatException>(() => content.GetFromString(playlistString));

			playlistString = "#EXTM3U";
			Assert.ThrowsException<FormatException>(() => content.GetFromString(playlistString));
		}

		[TestMethod]
		public void Throw_exception_when_create_master_from_media()
		{
			var content = new HlsMasterContent();

			var playlistString = "#EXTM3U\r\n#EXT-X-VERSION:1\r\n#EXTINF:-1,";
			Assert.ThrowsException<FormatException>(() => content.GetFromString(playlistString));
		}

		[TestMethod]
		public void Throw_exception_when_create_media_from_master()
		{
			var content = new HlsMediaContent();

			var playlistString = "#EXTM3U\r\n#EXT-X-VERSION:1\r\n#EXT-X-STREAM-INF:BANDWIDTH=124000";
			Assert.ThrowsException<FormatException>(() => content.GetFromString(playlistString));
		}

        [TestMethod]
        public void Round_trip_master_parse_totext()
        {
            var content = new HlsMasterContent();

            using (var stream = Helpers.ReadStream("PlaylistExtHlsMaster.m3u"))
            {
                string contents;
                using (StreamReader sr = new StreamReader(stream))
                {
                    contents = sr.ReadToEnd();
                }

                var playlist = content.GetFromString(contents);

                var toText = content.ToText(playlist);

                var file = content.GetFromString(toText);

                Assert.AreEqual("NO", file.AllowCache, false);
                Assert.AreEqual(1, file.Version);
                Assert.AreEqual(4, file.PlaylistEntries.Count);

                var entry = file.PlaylistEntries[0];
                Assert.AreEqual(1, entry.ProgramId);
                Assert.AreEqual(281600, entry.Bandwidth);
                Assert.AreEqual("mp4a.40.2", entry.Codecs[0]);
                Assert.AreEqual("HLS_9506_256k_v3/9506_256k_large_v3.m3u8", entry.Path);
            }
        }

		[TestMethod]
		public void Round_trip_media_parse_totext()
		{
            var content = new HlsMediaContent();

            using (var stream = Helpers.ReadStream("PlaylistExtHls.m3u"))
            {
				string contents;
				using (StreamReader sr = new StreamReader(stream))
				{
					contents = sr.ReadToEnd();
				}

				var playlist = content.GetFromString(contents);

				var toText = content.ToText(playlist);

				content = new HlsMediaContent();
				var file = content.GetFromString(toText);

                Assert.AreEqual("NO", file.AllowCache, false);
                Assert.AreEqual(1, file.Version);
                Assert.AreEqual(10, file.TargetDuration);
                Assert.AreEqual(56, file.PlaylistEntries.Count);

                var entry = file.PlaylistEntries[0];
                Assert.AreEqual(@"METHOD=AES-128,URI=""key""", entry.Key);
                Assert.AreEqual(10, entry.Duration);
                Assert.AreEqual(147483, entry.MediaSequence);
                Assert.AreEqual("stream1_256k_1_061532839410_00147483_v3.aac", entry.Path);

                entry = file.PlaylistEntries[1];
                Assert.AreEqual(@"METHOD=AES-128,URI=""key""", entry.Key);
                Assert.AreEqual(10, entry.Duration);
                Assert.AreEqual(147484, entry.MediaSequence);
                Assert.AreEqual("stream1_256k_1_061532849162_00147484_v3.aac", entry.Path);
            }
        }
	}
}
