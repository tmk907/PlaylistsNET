using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PlaylistsNET.Tests
{
    public class Helpers
    {
        public static string Read(string fileName)
        {
            Stream s = new FileStream(@"Examples/" + fileName, FileMode.Open);
            StreamReader tr = new StreamReader(s);
            string myText = tr.ReadToEnd();
            s.Dispose();
            tr.Dispose();
            return myText;
        }

        public static Stream ReadStream(string fileName)
        {
            Stream s = new FileStream(@"Examples/" + fileName, FileMode.Open);
            return s;
        }

        public static void Save(string fileName, string content)
        {
            Stream s = new FileStream(@"Examples/" + fileName, FileMode.Create);
            StreamWriter sw = new StreamWriter(s);
            sw.Write(content);
            sw.Flush();
            s.Dispose();
        }
    }
}
