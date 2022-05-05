using System;
using System.Collections.Generic;

namespace PlaylistsNET.Models
{
	public abstract class HlsPlaylist<T> : BasePlaylist<T> where T : HlsPlaylistEntry
	{
		public string Name { get; set; }
		public List<string> Comments { get; set; }
		public int Version { get; set; }
		[Obsolete("The EXT-X-ALLOW-CACHE tag was removed in protocol version 7.")]
		public string AllowCache { get; set; }

		public HlsPlaylist()
		{
			Comments = new List<string>();
		}
	}

	public class HlsMediaPlaylist : HlsPlaylist<HlsMediaPlaylistEntry>
	{
		public long MediaSequence { get; set; }
		public int? TargetDuration { get; set; }
		public long? DiscontinuitySequence { get; set; }
		public string PlaylistType { get; set; }
		public bool EndList { get; set; }
		public bool IFramesOnly { get; set; }

		public HlsMediaPlaylist() : base() { }
	}

	public class HlsMasterPlaylist : HlsPlaylist<HlsMasterPlaylistEntry>
	{
		public List<string> Media { get; set; }
		public List<string> IFrameStreamInf { get; set; }
		public List<string> SessionData { get; set; }
		public List<string> SessionKey { get; set; }

		public HlsMasterPlaylist() : base()
		{
			Media = new List<string>();
			IFrameStreamInf = new List<string>();
			SessionData = new List<string>();
			SessionData = new List<string>();
		}
	}
}
