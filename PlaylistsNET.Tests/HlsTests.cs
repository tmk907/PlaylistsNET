using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlaylistsNET.Content;
using PlaylistsNET.Models;
using System.Collections.Generic;
using System.Linq;

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
				Assert.AreEqual(1845, file.PlaylistEntries.Count);

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
	}
}
