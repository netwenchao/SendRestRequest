using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib
{
    public class FileUtils
    {
        public static void SaveFile(string filePath,string content)
        {
            var file = File.CreateText(filePath);
            file.Write(content);
            file.Close();
        }

        public static string GetFileContent(string filePath)
        {
            return File.Exists(filePath) ? File.ReadAllText(filePath) : "";
        }
    }
}
