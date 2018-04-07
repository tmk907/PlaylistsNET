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
        }

        //Test fails
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
    }
}
