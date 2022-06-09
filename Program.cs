using System;
using System.Collections.Generic;
using System.IO;

namespace SortFolderBySize
{
    class Program
    {
        public static string RootPath = @"";
        public static Dictionary<string, long> DirectoriesDictionary = new Dictionary<string, long>();

        static void Main(string[] args)
        {
            string[] directories = Directory.GetDirectories(RootPath);
            foreach (string directory in directories)
            {
                DirectoriesDictionary.Add(directory, GetFolderSize(directory));
                Console.WriteLine(directory);
            }

            foreach (var dic in DirectoriesDictionary)
            {
                Console.WriteLine(dic.Key + " " + dic.Value);
            }


        }

        public static long GetFolderSize(string folderPath)
        {
            long folderSize = 0;
            if(Directory.Exists(folderPath))
            {
                string[] folders = Directory.GetDirectories(folderPath);
                string[] files = Directory.GetFiles(folderPath);

                foreach (string directory in folders)
                {
                    folderSize += GetFolderSize(directory);
                }

                foreach (string file in files)
                {
                    var fileInfo = new FileInfo(file);
                    if(fileInfo.Exists) folderSize += fileInfo.Length;
                }
                
            }
            return folderSize;

        }
    }
}


