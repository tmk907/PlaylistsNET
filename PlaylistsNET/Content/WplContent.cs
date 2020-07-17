using System;
using System.IO;
using System.Text;
using PlaylistsNET.Models;
using System.Xml.Linq;

namespace PlaylistsNET.Content
{
    public class WplContent : IPlaylistParser<WplPlaylist>, IPlaylistWriter<WplPlaylist>
    {
        public string ToText(WplPlaylist playlist)
        {
            StringBuilder sb = new StringBuilder();
            XElement seq = CreateSeqWithMedia(playlist);
            XElement body = new XElement("body");
            body.Add(seq);
            XElement head = new XElement("head");
            if (!String.IsNullOrEmpty(playlist.Author))
            {
                XElement author = new XElement("author", playlist.Author);
                head.Add(author);
            }
            if (!String.IsNullOrEmpty(playlist.Guid))
            {
                XElement guid = new XElement("guid", playlist.Guid);
                head.Add(guid);
            }
            if (!String.IsNullOrEmpty(playlist.Generator))
            {
                head.Add(CreateMeta("Generator", playlist.Generator));
            }
            if (playlist.ItemCount > 0)
            {
                head.Add(CreateMeta("ItemCount", playlist.ItemCount.ToString()));
            }
            if (playlist.TotalDuration > TimeSpan.Zero)
            {
                head.Add(CreateMeta("totalDuration", ((int)playlist.TotalDuration.TotalMilliseconds).ToString()));
            }
            XElement title = new XElement("title", playlist.Title);
            head.Add(title);
            XElement smil = new XElement("smil");
            smil.Add(head);
            smil.Add(body);
            XDocument doc = new XDocument();
            doc.Add(smil);
            sb.AppendLine(@"<?wpl version=""1.0""?>");
            sb.Append(doc.ToString());
            return sb.ToString();
        }

		public WplPlaylist GetFromStream(Stream stream)
		{
			StreamReader streamReader = new StreamReader(stream);
			return GetFromString(streamReader.ReadToEnd());
		}

		public WplPlaylist GetFromString(string playlistString)
        {
            WplPlaylist playlist = new WplPlaylist();

			XDocument doc = XDocument.Parse(playlistString);
            XElement mainDocument = doc.Element("smil");
            XElement head = mainDocument.Element("head");
            playlist.Author = (string)head.Element("author") ?? "";
            playlist.Guid = (string)head.Element("guid") ?? "";
            playlist.Title = (string)head.Element("title") ?? "";
            var metaElements = head.Elements("meta");
            foreach(var metaElement in metaElements)
            {
                string name = Utils.Utils.UnEscape(metaElement.Attribute("name")?.Value);
                string content = Utils.Utils.UnEscape(metaElement.Attribute("content")?.Value);
                switch (name)
                {
                    case "Generator":
                        playlist.Generator = content;
                        break;
                    case "ItemCount":
                        int count = 0;
                        Int32.TryParse(content, out count);
                        playlist.ItemCount = count;
                        break;
                    case "totalDuration":
                        int miliseconds = 0;
                        Int32.TryParse(content, out miliseconds);
                        playlist.TotalDuration = TimeSpan.FromMilliseconds(miliseconds);
                        break;
                    default:
                        break;
                }
            }
            var mediaElements = mainDocument.Elements("body").Elements("seq").Elements("media");
            foreach (var media in mediaElements)
            {
                string src = Utils.Utils.UnEscape(media.Attribute("src")?.Value);
                string trackTitle = Utils.Utils.UnEscape(media.Attribute("trackTitle")?.Value);
                string trackArtist = Utils.Utils.UnEscape(media.Attribute("trackArtist")?.Value);
                string albumTitle = Utils.Utils.UnEscape(media.Attribute("albumTitle")?.Value);
                string albumArtist = Utils.Utils.UnEscape(media.Attribute("albumArtist")?.Value);
                int miliseconds = 0;
                Int32.TryParse(Utils.Utils.UnEscape(media.Attribute("duration")?.Value), out miliseconds);
                TimeSpan duration = TimeSpan.FromMilliseconds(miliseconds);
                playlist.PlaylistEntries.Add(new WplPlaylistEntry()
                {
                    AlbumArtist = albumArtist,
                    AlbumTitle = albumTitle,
                    Path = src,
                    TrackArtist = trackArtist,
                    TrackTitle = trackTitle
                });
            }

            return playlist;
        }
        
        public string Update(WplPlaylist playlist, Stream stream)
        {
            XDocument doc = XDocument.Load(stream);
            XElement mainDocument = doc.Element("smil");
            XElement head = mainDocument.Element("head");
            XElement title = head.Element("title");
            title.ReplaceWith(new XElement("title", playlist.Title));
            if (!String.IsNullOrEmpty(playlist.Guid))
            {
                XElement guid = head.Element("guid");
                guid.ReplaceWith(new XElement("guid", playlist.Guid));
            }
            if (!String.IsNullOrEmpty(playlist.Author))
            {
                XElement author = head.Element("author");
                author.ReplaceWith(new XElement("author", playlist.Author));
            }
            var meta = head.Elements("meta");
            foreach(var metaElement in meta)
            {
                string name = Utils.Utils.UnEscape(metaElement.Attribute("name")?.Value);
                string content = Utils.Utils.UnEscape(metaElement.Attribute("content")?.Value);
                switch (name)
                {
                    case "Generator":
                        if (!String.IsNullOrEmpty(playlist.Generator))
                        {
                            metaElement.SetAttributeValue("content", playlist.Generator);
                        }
                        break;
                    case "ItemCount":
                        if (playlist.ItemCount > 0)
                        {
                            metaElement.SetAttributeValue("content", playlist.ItemCount);
                        }
                        break;
                    case "totalDuration":
                        if (playlist.TotalDuration > TimeSpan.Zero)
                        {
                            metaElement.SetAttributeValue("content", (int)playlist.TotalDuration.TotalMilliseconds);
                        }
                        break;
                    default:
                        break;
                }
            }
            var seq = mainDocument.Elements("body").Elements("seq");
            XElement seqWithMedia = null;
            foreach(var s in seq)
            {
                var m3 = s.Elements("media");
                int i = 0;
                foreach(var a in m3) { i++; }
                if (i > 0)
                {
                    seqWithMedia = s;
                    break;
                }
            }
            if (seqWithMedia != null)
            {
                var newSeq = CreateSeqWithMedia(playlist);
                seqWithMedia.ReplaceWith(newSeq);
            }
            
            return doc.ToString();
        }

        private XElement CreateSeqWithMedia(WplPlaylist playlist)
        {
            XElement seq = new XElement("seq");
            foreach (var entry in playlist.PlaylistEntries)
            {
                XElement media = new XElement("media");
                XAttribute src = new XAttribute("src", entry.Path);
                media.Add(src);
                if (!String.IsNullOrEmpty(entry.AlbumArtist))
                {
                    XAttribute att = new XAttribute("albumTitle", entry.AlbumTitle);
                    media.Add(att);
                }
                if (!String.IsNullOrEmpty(entry.AlbumArtist))
                {
                    XAttribute att = new XAttribute("albumArtist", entry.AlbumArtist);
                    media.Add(att);
                }
                if (!String.IsNullOrEmpty(entry.TrackTitle))
                {
                    XAttribute att = new XAttribute("trackTitle", entry.TrackTitle);
                    media.Add(att);
                }
                if (!String.IsNullOrEmpty(entry.TrackArtist))
                {
                    XAttribute att = new XAttribute("trackArtist", entry.TrackArtist);
                    media.Add(att);
                }
                if (entry.Duration!=null && entry.Duration != TimeSpan.Zero)
                {
                    XAttribute att = new XAttribute("duration", (int)entry.Duration.TotalMilliseconds);
                    media.Add(att);
                }
                seq.Add(media);
            }
            return seq;
        }

        private XElement CreateMeta(string name, string content)
        {
            XElement meta = new XElement("meta");
            XAttribute attName = new XAttribute("name", name);
            XAttribute attContent = new XAttribute("content", content);
            meta.Add(attName);
            meta.Add(attContent);
            return meta;
        }
    }
}
