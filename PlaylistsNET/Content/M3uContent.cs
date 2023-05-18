﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PlaylistsNET.Models;

namespace PlaylistsNET.Content
{
    public class M3uContent : IPlaylistParser<M3uPlaylist>, IPlaylistWriter<M3uPlaylist>
    {
        public string ToText(M3uPlaylist playlist)
        {
            StringBuilder sb = new StringBuilder();

            if (playlist.IsExtended)
            {
                sb.AppendLine("#EXTM3U");
            }

            foreach (var currentComment in playlist.Comments)
            {
                sb.AppendLine($"#{currentComment}");
            }

            foreach (var entry in playlist.PlaylistEntries)
            {
                if (playlist.IsExtended)
                {
                    foreach (var currentComment in entry.Comments)
                    {
                        sb.AppendLine($"#{currentComment}");
                    }
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
                        foreach (var customProperty in entry.CustomProperties.Where(x => !string.IsNullOrEmpty(x.Value)))
                        {
                            sb.AppendLine($"#{customProperty.Key}:{customProperty.Value}");
                        }
                    }
                    sb.AppendLine($"#EXTINF:{(int)entry.Duration.TotalSeconds},{entry.Title}");
                }
                sb.AppendLine(entry.Path);
            }

            return sb.ToString().Trim();
        }

        public M3uPlaylist GetFromStream(Stream stream)
        {
            StreamReader streamReader = new StreamReader(stream);
            return GetFromString(streamReader.ReadToEnd());
        }

        public M3uPlaylist GetFromString(string playlistString)
        {
            var playlistLines = playlistString.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();

            // Not an EXT playlist, so parse with the standard parser
            if (playlistLines[0] != "#EXTM3U")
            {
                return GetM3u(playlistLines);
            }

            // Remove "#EXTM3U" as it is no longer needed
            playlistLines.RemoveAt(0);

            // EXT playlist, but not HLS playlist, parse with the EXT parser
            var isHls = playlistLines.Any(x => Regex.IsMatch(x, @"^#EXT-X-VERSION:\d$"));
            if (!isHls)
            {
                return GetExtM3u(playlistLines);
            }

            throw new FormatException("Playlist appears to be a HLS playlist. Use the HLS parser instead.");
        }

        private M3uPlaylist GetM3u(IEnumerable<string> playlistLines)
        {
            var playlist = new M3uPlaylist();

            foreach (var currentLine in playlistLines)
            {
                var Match = Regex.Match(currentLine, @"^#(.*)$");
                if (Match.Success)
                {
                    playlist.Comments.Add(currentLine);
                    continue;
                }

                playlist.PlaylistEntries.Add(new M3uPlaylistEntry()
                {
                    Path = currentLine,
                    Title = "",
                    Album = "",
                    AlbumArtist = "",
                });
            }

            return playlist;
        }

        private M3uPlaylist GetExtM3u(IEnumerable<string> playlistLines)
        {
            var playlist = new M3uPlaylist
            {
                IsExtended = true,
            };

            var currentEntry = new M3uPlaylistEntry {Album = "", AlbumArtist = "", Title = ""};
            foreach (var currentLine in playlistLines)
            {
                var match = Regex.Match(currentLine, @"^#EXTINF:(-?\d*),(.*)$");
                if (match.Success)
                {
                    var seconds = string.IsNullOrEmpty(match.Groups[1].Value) ? 0 : double.Parse(match.Groups[1].Value);
                    currentEntry.Duration = TimeSpan.FromSeconds(seconds);
                    currentEntry.Title = match.Groups[2].Value;
                    continue;
                }

                match = Regex.Match(currentLine, @"^#(EXTALB):(.*)$");
                if (match.Success)
                {
                    currentEntry.Album = match.Groups[2].Value;
                    continue;
                }

                match = Regex.Match(currentLine, @"^#(EXTART):(.*)$");
                if (match.Success)
                {
                    currentEntry.AlbumArtist = match.Groups[2].Value;
                    continue;
                }

                match = Regex.Match(currentLine, @"^#(EXT.*):(.*)$");
                if (match.Success)
                {
                    currentEntry.CustomProperties.Add(match.Groups[1].Value, match.Groups[2].Value);
                    continue;
                }

                match = Regex.Match(currentLine, @"^#(?!EXT)(.*)$");
                if (match.Success)
                {
                    currentEntry.Comments.Add(match.Groups[1].Value);
                    continue;
                }

                currentEntry.Path = Utils.Utils.DecodePath(currentLine);
                playlist.PlaylistEntries.Add(currentEntry);
                currentEntry = new M3uPlaylistEntry();
                currentEntry.Album = "";
                currentEntry.AlbumArtist = "";
                currentEntry.Title = "";
            }

            return playlist;
        }
    }
}
