using System;
using System.Collections.Generic;
using System.Text;

namespace PlaylistsNET.Models
{
	public abstract class HlsPlaylistEntry : BasePlaylistEntry
	{
		public Dictionary<string, string> CustomProperties { get; set; }
		public List<string> Comments { get; set; }

		public HlsPlaylistEntry()
		{
			CustomProperties = new Dictionary<string, string>();
			Comments = new List<string>();
		}

        protected abstract void CheckAndAppend<T>(string tag,
			T element,
			StringBuilder sb,
			Func<T, bool> validator);

        protected void CheckNullAndAppend<T>(string tag,
            T element,
            StringBuilder sb)
        {
            CheckAndAppend(tag, element, sb, e => e != null);
        }

        protected void CheckEmptyStringAndAppend(string tag,
            string element,
            StringBuilder sb)
        {
            CheckAndAppend(tag, element, sb, e => !String.IsNullOrEmpty(e));
        }
    }

	public class HlsMediaPlaylistEntry : HlsPlaylistEntry
	{
		public int Duration { get; set; }
		public string Title { get; set; }
		public long MediaSequence { get; set; }
		public bool Discontinuity { get; set; }
		public string ByteRange { get; set; }
		public string Key { get; set; }
		public string Map { get; set; }
		public DateTime? ProgramDateTime { get; set; }

		public HlsMediaPlaylistEntry() : base() { }

        protected override void CheckAndAppend<T>(string tag,
			T element,
			StringBuilder sb,
			Func<T, bool> validator)
        {
            if (validator(element) == true)
            {
                sb.AppendLine($"{tag}:{element}");
            }
		}

		public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

			CheckEmptyStringAndAppend("#EXT-X-BYTERANGE", ByteRange, sb);
			CheckNullAndAppend("#EXT-X-PROGRAM-DATE-TIME", ProgramDateTime?.ToString("o"), sb);

			if(Discontinuity)
			{
				sb.AppendLine("#EXT-X-DISCONTINUITY");
			}

			string durationTitle = String.Join(",",
				new string[] { Duration == 0 ? "" : Duration.ToString(), Title });
			CheckAndAppend("#EXTINF", durationTitle, sb, dt => !dt.Equals(","));

			foreach(var kv in CustomProperties)
			{
				sb.AppendLine($"#EXT{kv.Key}:{kv.Value}");
			}

			Comments.ForEach(comment => sb.AppendLine($"#{comment}"));

			sb.AppendLine(Path.TrimEnd());

			return sb.ToString();
        }
    }

	public class HlsMasterPlaylistEntry : HlsPlaylistEntry
	{
		[Obsolete("The PROGRAM-ID attribute of the EXT-X-STREAM-INF tag was removed in protocol version 6.")]
		public int? ProgramId { get; set; }
		public int Bandwidth { get; set; }
		public int? AverageBandwidth { get; set; }
		public List<string> Codecs { get; set; }
		public string Resolution { get; set; }
		public double? FrameRate { get; set; }
		public string HdcpLevel { get; set; }
		public string Audio { get; set; }
		public string Video { get; set; }
		public string Subtitles { get; set; }
		public string ClosedCaptions { get; set; }

		public HlsMasterPlaylistEntry() : base()
		{
			Codecs = new List<string>();
		}

        protected override void CheckAndAppend<T>(string tag,
			T element,
			StringBuilder sb,
			Func<T, bool> validator)
        {
            if (validator(element) == true)
            {
                sb.Append($"{tag}={element},");
            }
        }

        public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("#EXT-X-STREAM-INF:");
			CheckNullAndAppend("PROGRAM-ID", ProgramId, sb);
			sb.Append($"BANDWIDTH={Bandwidth},");
            CheckNullAndAppend("AVERAGE-BANDWIDTH", AverageBandwidth, sb);
            CheckEmptyStringAndAppend("CODECS", $"\"{String.Join(",", Codecs)}\"", sb);
            CheckEmptyStringAndAppend("RESOLUTION", Resolution, sb);
            CheckNullAndAppend("FRAME-RATE", FrameRate, sb);
            CheckEmptyStringAndAppend("HDCP-LEVEL", HdcpLevel, sb);
            CheckEmptyStringAndAppend("AUDIO", Audio, sb);
            CheckEmptyStringAndAppend("VIDEO", Video, sb);
            CheckEmptyStringAndAppend("SUBTITLES", Subtitles, sb);
            CheckEmptyStringAndAppend("CLOSED-CAPTIONS", ClosedCaptions, sb);

			int last = sb.Length - 1;
			if(sb[last] == ',')
			{
				sb.Remove(last, 1);
			}

			sb.AppendLine();
			sb.AppendLine(Path);

			return sb.ToString();
		}
	}
}
