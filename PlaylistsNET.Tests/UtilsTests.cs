using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PlaylistsNET.Tests
{
    [TestClass]
    public class UtilsTests
    {
        [TestMethod]
        public void MakeAbsolutePath_Equal()
        {
            string expectedPath = @"D:\Muzyka\Vanessa Mee\Contradanza.mp3";

            string folderPath = @"D:\Muzyka\Vanessa Mee";
            string filePath = "Contradanza.mp3";
            string path = Utils.Utils.MakeAbsolutePath(folderPath, filePath);
            Assert.AreEqual(path, expectedPath);

            folderPath = @"D:\Muzyka\Vanessa Mee";
            filePath = @"\Contradanza.mp3";
            path = Utils.Utils.MakeAbsolutePath(folderPath, filePath);
            Assert.AreEqual(path, expectedPath);

            folderPath = @"D:\Muzyka\Vanessa Mee\";
            filePath = "Contradanza.mp3";
            path = Utils.Utils.MakeAbsolutePath(folderPath, filePath);
            Assert.AreEqual(path, expectedPath);

            folderPath = @"D:\Muzyka\Vanessa Mee\";
            filePath = @"\Contradanza.mp3";
            path = Utils.Utils.MakeAbsolutePath(folderPath, filePath);
            Assert.AreEqual(path, expectedPath);

            folderPath = @"D:\Muzyka\Vanessa Mee";
            filePath = @".\Contradanza.mp3";
            path = Utils.Utils.MakeAbsolutePath(folderPath, filePath);
            Assert.AreEqual(path, expectedPath);

            folderPath = @"D:\Muzyka\Vanessa Mee\";
            filePath = @".\Contradanza.mp3";
            path = Utils.Utils.MakeAbsolutePath(folderPath, filePath);
            Assert.AreEqual(path, expectedPath);

            folderPath = @"D:\Muzyka";
            filePath = @".\Vanessa Mee\Contradanza.mp3";
            path = Utils.Utils.MakeAbsolutePath(folderPath, filePath);
            Assert.AreEqual(path, expectedPath);

            folderPath = @"D:\";
            filePath = @"Muzyka\Vanessa Mee\Contradanza.mp3";
            path = Utils.Utils.MakeAbsolutePath(folderPath, filePath);
            Assert.AreEqual(path, expectedPath);
        

            expectedPath = @"D:\Muzyka\Contradanza.mp3";

            folderPath = @"D:\Muzyka\Vanessa Mee";
            filePath = @"..\Contradanza.mp3";
            path = Utils.Utils.MakeAbsolutePath(folderPath, filePath);
            Assert.AreEqual(path, expectedPath);

            folderPath = @"D:\Muzyka\Vanessa Mee\";
            filePath = @"..\Contradanza.mp3";
            path = Utils.Utils.MakeAbsolutePath(folderPath, filePath);
            Assert.AreEqual(path, expectedPath);

            folderPath = @"D:\Muzyka\Vanessa Mee\CD2";
            filePath = @"..\..\Contradanza.mp3";
            path = Utils.Utils.MakeAbsolutePath(folderPath, filePath);
            Assert.AreEqual(path, expectedPath);

            folderPath = @"D:\Muzyka\Vanessa Mee\";
            filePath = @"..\..\Muzyka\Contradanza.mp3";
            path = Utils.Utils.MakeAbsolutePath(folderPath, filePath);
            Assert.AreEqual(path, expectedPath);
        }

        [TestMethod]
        public void MakeAbsolutePathForStream_Equal()
        {
            string expectedPath = @"http://music.com/top25/song.mp3";

            string folderPath = @"http://music.com/top25/";
            string filePath = @"/song.mp3";
            string path = Utils.Utils.MakeAbsolutePath(folderPath, filePath);
            Assert.AreEqual(path, expectedPath);

            folderPath = @"http://music.com/top25";
            filePath = @"/song.mp3";
            path = Utils.Utils.MakeAbsolutePath(folderPath, filePath);
            Assert.AreEqual(path, expectedPath);

            folderPath = @"http://music.com/top25/";
            filePath = @"song.mp3";
            path = Utils.Utils.MakeAbsolutePath(folderPath, filePath);
            Assert.AreEqual(path, expectedPath);

            folderPath = @"http://music.com/top25";
            filePath = @"song.mp3";
            path = Utils.Utils.MakeAbsolutePath(folderPath, filePath);
            Assert.AreEqual(path, expectedPath);
        }

        [TestMethod]
        public void MakeRelativePath_Equal()
        {
            string folderPath = @"D:\Muzyka\Vanessa Mee";
            string filePath = @"D:\Muzyka\Vanessa Mee\Contradanza.mp3";
            string expectedPath = @"Contradanza.mp3";
            string path = Utils.Utils.MakeRelativePath(folderPath, filePath);
            Assert.AreEqual(path, expectedPath);

            folderPath = @"D:\Muzyka\Vanessa Mee\";
            filePath = @"D:\Muzyka\Vanessa Mee\Contradanza.mp3";
            expectedPath = @"Contradanza.mp3";
            path = Utils.Utils.MakeRelativePath(folderPath, filePath);
            Assert.AreEqual(path, expectedPath);

            folderPath = @"D:\Muzyka";
            filePath = @"D:\Muzyka\Vanessa Mee\Contradanza.mp3";
            expectedPath = @"Vanessa Mee\Contradanza.mp3";
            path = Utils.Utils.MakeRelativePath(folderPath, filePath);
            Assert.AreEqual(path, expectedPath);

            folderPath = @"D:\Muzyka\Vanessa Mee\Folder1";
            filePath = @"D:\Muzyka\Vanessa Mee\Contradanza.mp3";
            expectedPath = @"..\Contradanza.mp3";
            path = Utils.Utils.MakeRelativePath(folderPath, filePath);
            Assert.AreEqual(path, expectedPath);

            folderPath = @"D:\Muzyka\Vanessa Mee\Folder1\Folder2";
            filePath = @"D:\Muzyka\Vanessa Mee\Contradanza.mp3";
            expectedPath = @"..\..\Contradanza.mp3";
            path = Utils.Utils.MakeRelativePath(folderPath, filePath);
            Assert.AreEqual(path, expectedPath);

            folderPath = @"D:\Muzyka\Vanessa Mee";
            filePath = @"D:\Muzyka\Other\Contradanza.mp3";
            expectedPath = @"..\Other\Contradanza.mp3";
            path = Utils.Utils.MakeRelativePath(folderPath, filePath);
            Assert.AreEqual(path, expectedPath);

            folderPath = @"D:\Muzyka\Vanessa Mee\Folder1";
            filePath = @"D:\Muzyka\Other1\Other2\Contradanza.mp3";
            expectedPath = @"..\..\Other1\Other2\Contradanza.mp3";
            path = Utils.Utils.MakeRelativePath(folderPath, filePath);
            Assert.AreEqual(path, expectedPath);
        }

        [TestMethod]
        [DataRow(@"D:\Muzyka\Andrea Bocelli\04 E Chiove.mp3", @"D:\Muzyka\Andrea Bocelli\04 E Chiove.mp3")]
        [DataRow("HLS_9506_256k_v3/9506_256k_large_v3.m3u8", "HLS_9506_256k_v3/9506_256k_large_v3.m3u8")]
        [DataRow(@".\C+C Music\01 - (Dance Now).mp3", @".\C+C Music\01 - (Dance Now).mp3")]
        [DataRow("Aimer/%E6%98%A5%E3%81%AF%E3%82%86%E3%81%8F%20-%20marie/01%20-%20%E6%98%A5", "Aimer/春はゆく - marie/01 - 春")]
        [DataRow("Aimer+/%E6%98%A5%E3%81%AF%E3%82%86%E3%81%8F%20-%20marie/01%20-%20%E6%98%A5", "Aimer /春はゆく - marie/01 - 春")]
        [DataRow("title c++.mp3", "title c++.mp3")]
        [DataRow("title%20.mp3", "title .mp3")]
        [DataRow("title%.mp3", "title%.mp3")]
        [DataRow("%A%B.mp3", "%A%B.mp3")]
        [DataRow("%FG.mp3", "%FG.mp3")]
        public void DecodePath_Equal(string path, string expectedPath)
        {
            var decodedPath = Utils.Utils.DecodePath(path);

            Assert.AreEqual(expectedPath, decodedPath);
        }

        [TestMethod]
        [DataRow("%01.mp3", "%01.mp3")]
        [DataRow("%A1.mp3", "%A1.mp3")]
        [DataRow("%FF.mp3", "%FF.mp3")]
        public void DecodePath_NotEqual(string path, string expectedPath)
        {
            var decodedPath = Utils.Utils.DecodePath(path);

            Assert.AreNotEqual(expectedPath, decodedPath);
        }
    }
}
