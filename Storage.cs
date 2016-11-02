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
        static string currentTimeStamp = DateTime.Now.ToString("ddMMyyyy");
        static string currentFile;
        static string lastMessageTimeStamp = "Not initialized yet ";

        public static void StorageInit() // Check does folders and files exist, and create them if not
        {
            bool IsFileCreated = false;
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
                    IsFileCreated = true;
                }
            }
            currentFile = path + GetFolderTimeStamp() + @".txt"; // Pointer to the last file
            if (IsFileCreated)
            {
                using (StreamWriter outputFile = new StreamWriter(currentFile, true))
                {
                    outputFile.WriteLine("Last event arrived at: " + lastMessageTimeStamp + "\r\n\r\n");
                }
            }
        }

        private static string GetFolderTimeStamp()
        {
            string s = DateTime.Now.ToString("ddMMyyyy");
            
            return s;
        }

        public static void UpdateFolderTimeStamp()
        {
            string temp = DateTime.Now.ToString("ddMMyyyy"); // Check is it new day
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
                    lastMessageTimeStamp = GetFolderTimeStamp();
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
