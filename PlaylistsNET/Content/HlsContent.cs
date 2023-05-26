﻿using PlaylistsNET.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PlaylistsNET.Content
{
    public class HlsMediaContent : HlsMediaWriter,
        IPlaylistParser<HlsMediaPlaylist>,
        IPlaylistWriter<HlsMediaPlaylist>
    {
        public string ToText(HlsMediaPlaylist playlist)
        {
            StringBuilder sb = new StringBuilder();

            AppendGeneralHeaders<HlsMediaPlaylist, HlsMediaPlaylistEntry>(playlist, ref sb);

            // Media playlist tags only
            CheckNullAndAppend("#EXT-X-TARGETDURATION", playlist.TargetDuration, sb);
            sb.AppendLine($"#EXT-X-MEDIA-SEQUENCE:{playlist.MediaSequence}");
            CheckNullAndAppend("#EXT-X-DISCONTINUITY-SEQUENCE", playlist.DiscontinuitySequence, sb);
            CheckEmptyStringAndAppend("#EXT-X-PLAYLIST-TYPE", playlist.PlaylistType, sb);

            if (playlist.IFramesOnly)
            {
                sb.AppendLine("#EXT-X-I-FRAMES-ONLY");
            }

            string key = "";
            string map = "";

            foreach (HlsMediaPlaylistEntry entry in playlist.PlaylistEntries)
            {
                if (!String.IsNullOrEmpty(entry.Key) && !entry.Key.Equals(key))
                {
                    sb.AppendLine($"#EXT-X-KEY:{entry.Key}");
                    key = entry.Key;
                }

                if (!String.IsNullOrEmpty(entry.Map) && !entry.Map.Equals(map))
                {
                    sb.AppendLine($"#EXT-X-MAP:{entry.Map}");
                    map = entry.Map;
                }

                sb.Append(entry.ToString());
            }

            if (playlist.EndList)
            {
                sb.AppendLine("#EXT-X-ENDLIST");
            }

            return sb.ToString();
        }
        public HlsMediaPlaylist GetFromStream(Stream stream)
        {
            StreamReader streamReader = new StreamReader(stream);
            return GetFromString(streamReader.ReadToEnd());
        }

        public HlsMediaPlaylist GetFromStream(Stream stream, Encoding encoding)
        {
            StreamReader streamReader = new StreamReader(stream, encoding);
            return GetFromString(streamReader.ReadToEnd());
        }

        public HlsMediaPlaylist GetFromString(string playlistString)
        {
            var playlistLines = playlistString.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();

            // Not an EXT playlist, so can't be an HLS playlist
            if (playlistLines[0] != "#EXTM3U")
            {
                throw new FormatException("Playlist missing required EXTM3U tag.");
            }

            // Remove "#EXTM3U" as it is no longer needed
            playlistLines.RemoveAt(0);

            // EXT playlist, but not HLS playlist, parse with the EXT parser
            var isHls = playlistLines.Where(x => Regex.IsMatch(x, @"^#EXT-X-VERSION:\d$")).Any();
            if (!isHls)
            {
                throw new FormatException("Playlist missing required EXT-X-VERSION tag.");
            }

            // HLS Master, not Media
            var isMaster = playlistLines.Where(x => Regex.IsMatch(x, @"^#EXT-X-STREAM-INF:.+$")).Any();
            isMaster = isMaster || playlistLines.Where(x => Regex.IsMatch(x, @"^#EXT-X-MEDIA:(.*)$")).Any();
            isMaster = isMaster || playlistLines.Where(x => Regex.IsMatch(x, @"^#EXT-X-I-FRAME-STREAM-INF:(.*)$")).Any();
            isMaster = isMaster || playlistLines.Where(x => Regex.IsMatch(x, @"^#EXT-X-SESSION-DATA:(.*)$")).Any();
            isMaster = isMaster || playlistLines.Where(x => Regex.IsMatch(x, @"^#EXT-X-SESSION-KEY:(.*)$")).Any();
            if (isMaster)
            {
                throw new FormatException("Playlist appears to be a HLS Master playlist.");
            }

            return GetMediaHls(playlistLines);
        }

        private HlsMediaPlaylist GetMediaHls(List<string> playlistLines)
        {
            var playlist = new HlsMediaPlaylist();
            var currentEntry = new HlsMediaPlaylistEntry();
            foreach (var currentLine in playlistLines)
            {
                // Media and Master tags
                var hlsMatch = Regex.Match(currentLine, @"^#EXT-X-VERSION:(\d*)$");
                if (hlsMatch.Success)
                {
                    playlist.Version = int.Parse(hlsMatch.Groups[1].Value);
                    continue;
                }

                hlsMatch = Regex.Match(currentLine, @"^#EXT-X-ALLOW-CACHE:(.*)$");
                if (hlsMatch.Success)
                {
                    playlist.AllowCache = hlsMatch.Groups[1].Value;
                    continue;
                }

                // Media playlist tags only
                hlsMatch = Regex.Match(currentLine, @"^#EXT-X-TARGETDURATION:(\d*)$");
                if (hlsMatch.Success)
                {
                    playlist.TargetDuration = int.Parse(hlsMatch.Groups[1].Value);
                    continue;
                }

                hlsMatch = Regex.Match(currentLine, @"^#EXT-X-MEDIA-SEQUENCE:(\d*)$");
                if (hlsMatch.Success)
                {
                    var mediaSequence = long.Parse(hlsMatch.Groups[1].Value);
                    playlist.MediaSequence = mediaSequence;
                    currentEntry.MediaSequence = mediaSequence;
                    continue;
                }

                hlsMatch = Regex.Match(currentLine, @"^#EXT-X-DISCONTINUITY-SEQUENCE:(\d*)$");
                if (hlsMatch.Success)
                {
                    playlist.DiscontinuitySequence = long.Parse(hlsMatch.Groups[1].Value);
                    continue;
                }

                hlsMatch = Regex.Match(currentLine, @"^EXT-X-PLAYLIST-TYPE:(.*)$");
                if (hlsMatch.Success)
                {
                    playlist.PlaylistType = hlsMatch.Groups[1].Value;
                    continue;
                }

                hlsMatch = Regex.Match(currentLine, @"^#EXT-X-ENDLIST$");
                if (hlsMatch.Success)
                {
                    playlist.EndList = true;
                    continue;
                }

                hlsMatch = Regex.Match(currentLine, @"^#EXT-X-I-FRAMES-ONLY$");
                if (hlsMatch.Success)
                {
                    playlist.IFramesOnly = true;
                    continue;
                }

                // Per entry tags
                hlsMatch = Regex.Match(currentLine, @"^#EXT-X-KEY:(.*)$");
                if (hlsMatch.Success)
                {
                    currentEntry.Key = hlsMatch.Groups[1].Value;
                    continue;
                }

                hlsMatch = Regex.Match(currentLine, @"^#EXT-X-MAP:(.*)$");
                if (hlsMatch.Success)
                {
                    currentEntry.Map = hlsMatch.Groups[1].Value;
                    continue;
                }

                hlsMatch = Regex.Match(currentLine, @"^#EXT-X-BYTERANGE:(.*)$");
                if (hlsMatch.Success)
                {
                    currentEntry.ByteRange = hlsMatch.Groups[1].Value;
                    continue;
                }

                hlsMatch = Regex.Match(currentLine, @"^#EXT-X-PROGRAM-DATE-TIME:(.*)$");
                if (hlsMatch.Success)
                {
                    currentEntry.ProgramDateTime = DateTime.Parse(hlsMatch.Groups[1].Value).ToUniversalTime();
                    continue;
                }

                hlsMatch = Regex.Match(currentLine, @"^#(EXT-X-DISCONTINUITY)$");
                if (hlsMatch.Success)
                {
                    currentEntry.Discontinuity = true;
                    continue;
                }

                var match = Regex.Match(currentLine, @"^#EXTINF:(-?\d*),(.*)$");
                if (match.Success)
                {
                    currentEntry.Duration = string.IsNullOrEmpty(match.Groups[1].Value) ? 0 : int.Parse(match.Groups[1].Value);
                    currentEntry.Title = match.Groups[2].Value;
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

                currentEntry.Path = currentLine;
                playlist.PlaylistEntries.Add(currentEntry);

                // Key and Map apply to all subsequent entries until another Key or Map tag is specified
                currentEntry = new HlsMediaPlaylistEntry()
                {
                    MediaSequence = currentEntry.MediaSequence + 1,
                    Key = currentEntry.Key,
                    Map = currentEntry.Map
                };
            }

            return playlist;
        }
    }

    public class HlsMasterContent : HlsMediaWriter,
        IPlaylistParser<HlsMasterPlaylist>,
        IPlaylistWriter<HlsMasterPlaylist>
    {
        public string ToText(HlsMasterPlaylist playlist)
        {
            StringBuilder sb = new StringBuilder();

            AppendGeneralHeaders<HlsMasterPlaylist, HlsMasterPlaylistEntry>(playlist, ref sb);

            // Master playlist-wide tags
            playlist.Media.ForEach(media =>
            {
                sb.AppendLine($"#EXT-X-MEDIA:{media}");
            });

            playlist.IFrameStreamInf.ForEach(iFrameStreamInf =>
            {
                sb.AppendLine($"#EXT-X-I-FRAME-STREAM-INF:{iFrameStreamInf}");
            });

            playlist.SessionData.ForEach(sessionData =>
            {
                sb.AppendLine($"#EXT-X-SESSION-DATA:{sessionData}");
            });

            playlist.SessionKey.ForEach(sessionKey =>
            {
                sb.AppendLine($"#EXT-X-SESSION-KEY:{sessionKey}");
            });

            // Handle master playlist entries
            foreach (HlsMasterPlaylistEntry entry in playlist.PlaylistEntries)
            {
                sb.Append(entry.ToString());
            }

            return sb.ToString();
        }
        public HlsMasterPlaylist GetFromStream(Stream stream)
        {
            StreamReader streamReader = new StreamReader(stream);
            return GetFromString(streamReader.ReadToEnd());
        }

        public HlsMasterPlaylist GetFromStream(Stream stream, Encoding encoding)
        {
            StreamReader streamReader = new StreamReader(stream, encoding);
            return GetFromString(streamReader.ReadToEnd());
        }

        public HlsMasterPlaylist GetFromString(string playlistString)
        {
            var playlistLines = playlistString.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();

            // Not an EXT playlist, so can't be an HLS playlist
            if (playlistLines[0] != "#EXTM3U")
            {
                throw new FormatException("Playlist missing required EXTM3U tag.");
            }

            // Remove "#EXTM3U" as it is no longer needed
            playlistLines.RemoveAt(0);

            // EXT playlist, but not HLS playlist, parse with the EXT parser
            var isHls = playlistLines.Where(x => Regex.IsMatch(x, @"^#EXT-X-VERSION:\d$")).Any();
            if (!isHls)
            {
                throw new FormatException("Playlist missing required EXT-X-VERSION tag.");
            }

            // HLS Media playlist, not Master
            var isMedia = playlistLines.Where(x => Regex.IsMatch(x, @"^#EXTINF:.+$")).Any();
            isMedia = isMedia || playlistLines.Where(x => Regex.IsMatch(x, @"^#EXT-X-TARGETDURATION:(\d*)$")).Any();
            isMedia = isMedia || playlistLines.Where(x => Regex.IsMatch(x, @"^#EXT-X-MEDIA-SEQUENCE:(\d*)$")).Any();
            isMedia = isMedia || playlistLines.Where(x => Regex.IsMatch(x, @"^#EXT-X-DISCONTINUITY-SEQUENCE:(\d*)$")).Any();
            isMedia = isMedia || playlistLines.Where(x => Regex.IsMatch(x, @"^#EXT-X-PLAYLIST-TYPE:(.*)$")).Any();
            isMedia = isMedia || playlistLines.Where(x => Regex.IsMatch(x, @"^#EXT-X-ENDLIST$")).Any();
            isMedia = isMedia || playlistLines.Where(x => Regex.IsMatch(x, @"^#EXT-X-I-FRAMES-ONLY$")).Any();
            if (isMedia)
            {
                throw new FormatException("Playlist appears to be a HLS Media playlist.");
            }

            return GetMasterHls(playlistLines);
        }

        private HlsMasterPlaylist GetMasterHls(List<string> playlistLines)
        {
            var playlist = new HlsMasterPlaylist();
            var currentEntry = new HlsMasterPlaylistEntry();
            foreach (var currentLine in playlistLines)
            {
                // Media and Master tags
                var match = Regex.Match(currentLine, @"^#EXT-X-VERSION:(\d*)$");
                if (match.Success)
                {
                    playlist.Version = int.Parse(match.Groups[1].Value);
                    continue;
                }

                match = Regex.Match(currentLine, @"^#EXT-X-ALLOW-CACHE:(.*)$");
                if (match.Success)
                {
                    playlist.AllowCache = match.Groups[1].Value;
                    continue;
                }

                // Master playlist-wide tags
                match = Regex.Match(currentLine, @"^#EXT-X-MEDIA:(.*)$");
                if (match.Success)
                {
                    playlist.Media.Add(match.Groups[1].Value);
                    continue;
                }

                match = Regex.Match(currentLine, @"^#EXT-X-I-FRAME-STREAM-INF:(.*)$");
                if (match.Success)
                {
                    playlist.IFrameStreamInf.Add(match.Groups[1].Value);
                    continue;
                }

                match = Regex.Match(currentLine, @"^#EXT-X-SESSION-DATA:(.*)$");
                if (match.Success)
                {
                    playlist.SessionData.Add(match.Groups[1].Value);
                    continue;
                }

                match = Regex.Match(currentLine, @"^#EXT-X-SESSION-KEY:(.*)$");
                if (match.Success)
                {
                    playlist.SessionKey.Add(match.Groups[1].Value);
                    continue;
                }

                // Master playlist entry tags
                match = Regex.Match(currentLine, @"^#EXT-X-STREAM-INF:(.*)$");
                if (match.Success)
                {
                    var streamMatches = Regex.Matches(match.Groups[1].Value, @"([-A-Z]+)=(""[^""]+|[^,]+)");

                    foreach (Match currentMatch in streamMatches)
                    {
                        var value = currentMatch.Groups[2].Value.Trim('"');

                        switch (currentMatch.Groups[1].Value)
                        {
                            case "PROGRAM-ID":
                                currentEntry.ProgramId = int.Parse(value);
                                break;
                            case "BANDWIDTH":
                                currentEntry.Bandwidth = int.Parse(value);
                                break;
                            case "AVERAGE-BANDWIDTH":
                                currentEntry.AverageBandwidth = int.Parse(value);
                                break;
                            case "CODECS":
                                currentEntry.Codecs.AddRange(value.Split(','));
                                break;
                            case "RESOLUTION":
                                currentEntry.Resolution = value;
                                break;
                            case "FRAME-RATE":
                                currentEntry.FrameRate = double.Parse(value);
                                break;
                            case "HDCP-LEVEL":
                                currentEntry.HdcpLevel = value;
                                break;
                            case "AUDIO":
                                currentEntry.Audio = value;
                                break;
                            case "VIDEO":
                                currentEntry.Video = value;
                                break;
                            case "SUBTITLES":
                                currentEntry.Subtitles = value;
                                break;
                            case "CLOSED-CAPTIONS":
                                currentEntry.ClosedCaptions = value;
                                break;
                            default:
                                throw new FormatException($"STREAM-INF tag contains unknown attribute: {currentMatch.Groups[1].Value}");
                        }
                    }

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

                currentEntry.Path = currentLine;
                playlist.PlaylistEntries.Add(currentEntry);
                currentEntry = new HlsMasterPlaylistEntry();
            }

            return playlist;
        }
    }
    public class HlsMediaWriter
    {
        protected void AppendGeneralHeaders<T, U>(T playlist, ref StringBuilder sb)
            where T : HlsPlaylist<U>
            where U : HlsPlaylistEntry
        {
            // HLS requires this line as the first line of playlist
            sb.AppendLine("#EXTM3U");
            // VERSION always required
            sb.AppendLine($"#EXT-X-VERSION:{playlist.Version}");

            CheckNullAndAppend("#EXT-X-ALLOW-CACHE", playlist.AllowCache, sb);

            foreach (var currentComment in playlist.Comments)
            {
                sb.AppendLine($"#{currentComment}");
            }
        }
        protected void CheckAndAppend<T>(string tag,
            T element,
            StringBuilder sb,
            Func<T, bool> validator)
        {
            if (validator(element) == true)
            {
                sb.AppendLine($"{tag}:{element}");
            }
        }

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
}
