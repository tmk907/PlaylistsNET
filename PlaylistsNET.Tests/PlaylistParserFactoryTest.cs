using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlaylistsNET.Content;
using System;

namespace PlaylistsNET.Tests
{
    [TestClass]
    public class PlaylistParserFactoryTest
    {
        [TestMethod]
        public void Create_correct_parser_from_known_filetype()
        {
            var filetype = ".m3u";
            var parser = PlaylistParserFactory.GetPlaylistParser(filetype);

            Assert.IsNotNull(parser);
            Assert.IsInstanceOfType(parser, typeof(M3uContent));
        }

        [TestMethod]
        public void Throw_exception_when_create_parser_from_unknown_filetype()
        {
            var filetype = ".abc";

            Assert.ThrowsException<ArgumentException>(() => PlaylistParserFactory.GetPlaylistParser(filetype));
        }

        [TestMethod]
        public void Create_correct_parser_from_PlaylistType()
        {
            var playlistType = PlaylistType.M3U;
            var parser = PlaylistParserFactory.GetPlaylistParser(playlistType);

            Assert.IsNotNull(parser);
            Assert.IsInstanceOfType(parser, typeof(M3uContent));
        }
    }
}
