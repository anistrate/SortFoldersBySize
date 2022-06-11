using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace SortFolderBySize
{

    class Program
    {
        private static string desktopIniName = "desktop.ini";
        private static string RootPath = @"D:\Things to backup monthly\test";
        private static string Line1 = "[.ShellClassInfo]";
        private static string Line2 = "[{F29F85E0-4FF9-1068-AB91-08002B27B3D9}]";
        private static string Line3 = "Prop5=31,FolderTag";
        private static string MagicCommentForCreatedFiles = "; DangerCouldBeMyMiddleNameButItsJohn";
        private static string MagicCommentForAppendedFiles = "; WatchMrRobotNoW";


        private static Dictionary<string, long> DirectoriesDictionary = new Dictionary<string, long>();
       



        static void Main(string[] args)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            string[] directories = Directory.GetDirectories(RootPath);
            foreach (string directory in directories)
            {
                DirectoriesDictionary.Add(directory, GetFolderSize(directory));
            }

            stopwatch.Stop();
            var elapsedTime = stopwatch.ElapsedMilliseconds;
            Random random = new Random();
            foreach (var dic in DirectoriesDictionary)
            {
                //DeleteDesktopIniFile(dic.Key);
                CreateDesktopIniFile(dic.Key, random.Next(0, 1000000));
                Console.WriteLine(dic.Key + " " + dic.Value);
            }
            Console.WriteLine($"Elapsed time:{elapsedTime/1000} seconds");



        }

        private static long GetFolderSize(string folderPath)
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

        private static void CreateDesktopIniFile(string folder, long size)
        {
            string filePath = folder + "/" + desktopIniName;
            using (var stream = new StreamWriter(File.Create(filePath)))
            {
                stream.WriteLine(Line1);
                stream.WriteLine(Line2);
                stream.WriteLine(Line3.Replace("FolderTag", size.ToString()));
                stream.WriteLine(MagicCommentForCreatedFiles);
            }
            File.SetAttributes(folder, FileAttributes.ReadOnly);
        }

        private static void DeleteDesktopIniFile(string folder)
        {
            var fileShouldBeDeleted = false;
            string filePath = folder + "/" + desktopIniName;
            if(File.Exists(filePath))
            {
                using (var reader = new StreamReader(filePath))
                {
                    string contents = reader.ReadToEnd();
                    if (contents.Contains(MagicCommentForCreatedFiles)) fileShouldBeDeleted = true;
                    else if (contents.Contains(MagicCommentForAppendedFiles)) ;
                }

                if (fileShouldBeDeleted)
                {
                    File.SetAttributes(folder, FileAttributes.Normal);
                    File.Delete(filePath);
                }
            }
        }


    }
}


