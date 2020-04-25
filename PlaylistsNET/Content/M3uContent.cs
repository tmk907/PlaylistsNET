using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PlaylistsNET.Models;

namespace PlaylistsNET.Content
{
    public class M3uContent : IPlaylistParser<M3uPlaylist>, IPlaylistWriter<M3uPlaylist>
    {
        private const string EXTM3U = "#EXTM3U";
        private const string EXTINF = "#EXTINF";
        private const string EXTALB = "#EXTALB";
        private const string EXTART = "#EXTART";

        public string ToText(M3uPlaylist playlist)
        {
            StringBuilder sb = new StringBuilder();
            
            if (playlist.IsExtended)
            {
                sb.AppendLine("#EXTM3U");
            }
            foreach(var entry in playlist.PlaylistEntries)
            {
                if (playlist.IsExtended)
                {
                    if (!String.IsNullOrEmpty(entry.Album))
                    {
                        sb.Append("#EXTALB:").Append(entry.Album).AppendLine();
                    }
                    if (!String.IsNullOrEmpty(entry.AlbumArtist))
                    {
                        sb.Append("#EXTART:").Append(entry.AlbumArtist).AppendLine();
                    }
                    if (entry.CustomProperties != null)
                    {
                        foreach (var customProperty in entry.CustomProperties)
                        {
                            sb.Append('#').Append(customProperty.Key).Append(':').Append(customProperty.Value).AppendLine();
                        }
                    }
                    sb.Append("#EXTINF:").Append((int)entry.Duration.TotalSeconds).Append(',').Append(entry.Title).AppendLine();
                }
                sb.AppendLine(entry.Path);
            }
            sb = sb.Replace(Environment.NewLine, "", sb.Length - 3, 3);
            return sb.ToString();
        }

        public M3uPlaylist GetFromStream(Stream stream)
        {
            M3uPlaylist playlist = new M3uPlaylist();
            StreamReader streamReader = new StreamReader(stream);
            if (!streamReader.EndOfStream)
            {
                string header = streamReader.ReadLine().Trim();
                if (header == "#EXTM3U")
                {
                    playlist.IsExtended = true;
                }
                else
                {
                    playlist.IsExtended = false;
                    playlist.PlaylistEntries.Add(new M3uPlaylistEntry()
                    {
                        Duration = TimeSpan.Zero,
                        Path = header,
                        Title = ""
                    });
                }
            }
            bool prevLineIsExtInf = false;
            string title = "";
            string artist = "";
            string album = "";
            int seconds = 0;
            List<KeyValuePair<string, string>> customProperties = null;
            while (!streamReader.EndOfStream)
            {
                string line = streamReader.ReadLine();
                if (line.StartsWith("#"))
                {
                    if (playlist.IsExtended)
                    {
                        if (line.StartsWith("#EXTINF"))
                        {
                            prevLineIsExtInf = true;
                            title = GetTitle(line);
                            seconds = GetSeconds(line);
                        }
                        else if (line.StartsWith("#EXTALB"))
                        {
                            album = GetAlbum(line);
                        }
                        else if (line.StartsWith("#EXTART"))
                        {
                            artist = GetArtist(line);
                        }
                        else
                        {
                            var delimeterPosition = line.IndexOf(':');
                            if (delimeterPosition >= 0)
                            {
                                var key = line.Substring(1, delimeterPosition - 1);
                                var value = line.Substring(delimeterPosition + 1);
                                if (customProperties == null)
                                {
                                    customProperties = new List<KeyValuePair<string, string>>();
                                }
                                customProperties.Add(new KeyValuePair<string, string>(key, value));
                            }
                        }
                    }
                }
                else
                {
                    if (!playlist.IsExtended || !prevLineIsExtInf)
                    {
                        title = "";
                        artist = "";
                        album = "";
                        seconds = 0;
                    }
                    playlist.PlaylistEntries.Add(new M3uPlaylistEntry()
                    {
                        Album = album,
                        AlbumArtist = artist,
                        Duration = TimeSpan.FromSeconds(seconds),
                        Path = line,
                        Title = title,
                        CustomProperties = customProperties ?? Enumerable.Empty<KeyValuePair<string, string>>()
                    });
                    prevLineIsExtInf = false;
                    customProperties = null;
                }
            }
            return playlist;
        }

        private int GetSeconds(string line)
        {
            int seconds = 0;
            try
            {
                //0123456789
                //#EXTINF:1234,
                string s = line.Substring(8, line.IndexOf(',') - 8);
                seconds = (int)double.Parse(s);
            }
            catch { }
            return seconds;
        }

        private string GetTitle(string line)
        {
            string title = "";
            try
            {
                title = line.Substring(line.IndexOf(',') + 1);
            }
            catch { }
            return title;
        }

        private string GetAlbum(string line)
        {
            string album = "";
            try
            {
                album = line.Substring(line.IndexOf(':') + 1);
            }
            catch { }
            return album;
        }

        private string GetArtist(string line)
        {
            string artist = "";
            try
            {
                artist = line.Substring(line.IndexOf(':') + 1);
            }
            catch { }
            return artist;
        }
    }
}
