using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPServer
{
    class Storage
    {
        static string currentTimeStamp = DateTime.Now.ToString("dd/MM/yyyy");
        static string currentFile;

        public static void StorageInit() // Check does folders and files exist, and create them if not
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"log";
            
            if (!Directory.Exists(path))
            {
                DirectoryInfo di = Directory.CreateDirectory(path);
            }
            path +=  @"\";

            if (!File.Exists(path + GetFolderTimeStamp() + @".txt"))
            {
                using (File.Create(path + GetFolderTimeStamp() + @".txt"))
                {
                    Console.WriteLine("New file created");
                }
            }
            currentFile = path + GetFolderTimeStamp() + @".txt"; // Pointer to the last file
        }

        private static string GetFolderTimeStamp()
        {
            string s = DateTime.Now.ToString("dd/MM/yyyy");
            
            return s;
        }

        public static void UpdateFolderTimeStamp()
        {
            string temp = DateTime.Now.ToString("dd/MM/yyyy"); // Check is it new day
            if (!temp.Equals(currentTimeStamp))
            {
                currentTimeStamp = temp;
                StorageInit(); // Update folders
            }

        }

        public static void AppendTextToFile(string s)
        {
            // Append text to an existing file
            try
            {
                using (StreamWriter outputFile = new StreamWriter(currentFile, true))
                {
                    outputFile.WriteLine(s);
                }
            }
            catch (DirectoryNotFoundException)
            {
                StorageInit();
                using (StreamWriter outputFile = new StreamWriter(currentFile, true))
                {
                    outputFile.WriteLine(s);
                }

            }
            
        }

    }
}
