using SortFoldersBySize.Models;
using SortFoldersBySize.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
using static Vanara.PInvoke.Shell32;
//using Vanara;


namespace SortFolderBySize
{

    public class SortFolders
    {
        private static string desktopIniName = "desktop.ini";
        //private static string RootPath = @"D:\Things to backup monthly\test";


        private static string CalculateFolderSizeCommand = "c";
        private static string RemoveFolderTagsCommand = "r";


        private static string IncorrectNumberOfParameters = "Expected 2 arguments, found :";
        private static string PathDoesNotExist = "Path does not exist: ";
        private static string CorrectUsageFormat = "program <path> c|r";
        private static string InvalidCommand = " is not a valid command, please use c|r";
        private const string FilePathNotFound = "Filepath {0} not found";
        //private static Dictionary<string, long> DirectoriesDictionary = new Dictionary<string, long>();

        private readonly SizeCalculator sizeCalculator;
        private readonly CommandInterpreter interpreter = new CommandInterpreter();
        public SortFolders(IFileSystem fileSystem)
        {

            sizeCalculator = new SizeCalculator(fileSystem);
        }

        public Result<CommandArgs> InterpretCommand(string[] args)
        {
            return interpreter.InterpretCommand(args);
        }

        static void Main(string[] args)
        {
            var sortFolders = new SortFolders(new FileSystem());
            var argResult = sortFolders.InterpretCommand(args);

            if(argResult.IsFailure)
            {
                //TODO
                //to use windows tray notifications
                //also consider a log file
                Console.WriteLine(argResult.Error);
                return;
            }

            var command = argResult.Value;
            var folderResult = sortFolders.sizeCalculator.FolderExists(command.RootPath);

            if(folderResult.IsFailure)
            {
                //TODO
                //to use windows tray notifications
                //also consider a log file
                Console.WriteLine(folderResult.Error);
                return;
            }

            var commandResult = sortFolders.sizeCalculator.InterpretCommand(command);
            
            if(commandResult.IsFailure)
            {

            }

            //TODO add a -help or --help command??

            

            args = new string[2];
            args[0] = @"D:\Things to backup monthly\test";
            args[1] = "c";

            try
            { 
                if (args[1] == CalculateFolderSizeCommand)
                {
                    var directoriesDictionary = CalculateFolderSizes(RootPath);
                    AddFolderTags(directoriesDictionary);
                    ForceWindowsExplorerToShowTag(RootPath);

                }
                else if (args[1] == RemoveFolderTagsCommand)
                {
                    RemoveFolderTags(RootPath);
                    ForceWindowsExplorerToShowTag(RootPath);
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
            


        }

        private static void RemoveFolderTags(string path)
        {
            string[] directories = Directory.GetDirectories(path);
            foreach (string directory in directories)
            {
                RemoveTagForFolder(directory);
            }
        }

        private static void AddFolderTags(Dictionary<string, long> directoriesDictionary)
        {
            
            foreach (var dic in directoriesDictionary)
            {
                CreateDesktopIniFile(dic.Key, dic.Value);
            }
           
        }


        private static void CreateDesktopIniFile(string folder, long size)
        {
            string filePath = folder + "/" + desktopIniName;
            if (!File.Exists(filePath))
            {
                using (var stream = new StreamWriter(File.Create(filePath)))
                {
                    stream.WriteLine(Line1);
                    stream.WriteLine(Line2);
                    stream.WriteLine(Line3.Replace("FolderTag", FormatSizeinKB(size).ToString()));
                    stream.WriteLine(Line4.Replace("Title", FormatSizeinKB(size).ToString("N0") + " KB"));
                    stream.WriteLine(MagicCommentForCreatedFiles);
                }
                //ForceWindowsExplorerToShowTag(folder);
                File.SetAttributes(folder, FileAttributes.ReadOnly);
            }
            else
            {
                using (var reader = new StreamReader(filePath))
                {
                    string contents = reader.ReadToEnd();
                    if (contents.Contains(MagicCommentForCreatedFiles) || contents.Contains(MagicCommentForAppendedFiles))
                    {
                        File.WriteAllLines(filePath, File.ReadAllLines(filePath).
                             Where(x => !x.Contains("Prop5=31") ||
                                        !x.Contains("Prop2=31")));
                        

                        File.AppendAllText(filePath, Line3.Replace("FolderTag", FormatSizeinKB(size).ToString()));
                        File.AppendAllText(filePath,Line4.Replace("Title", FormatSizeinKB(size).ToString("N0") + " KB"));

                    }
                    else 
                    {
                        File.AppendAllText(filePath,Line3.Replace("FolderTag", FormatSizeinKB(size).ToString()));
                        File.AppendAllText(filePath,Line4.Replace("Title", FormatSizeinKB(size).ToString("N0") + " KB"));
                        File.AppendAllText(filePath,MagicCommentForAppendedFiles);
                    }
                }
            }
        }

        private static void RemoveTagForFolder(string folder)
        {
            var fileShouldBeDeleted = false;
            string filePath = folder + "/" + desktopIniName;
            if(File.Exists(filePath))
            {
                using (var reader = new StreamReader(filePath))
                {
                    string contents = reader.ReadToEnd();
                    if (contents.Contains(MagicCommentForCreatedFiles)) fileShouldBeDeleted = true;
                    else if (contents.Contains(MagicCommentForAppendedFiles))
                    {
                        File.WriteAllLines(filePath, File.ReadAllLines(filePath).
                             Where(x => !x.Contains("Prop5=31") ||
                                        !x.Contains(MagicCommentForAppendedFiles) ||
                                        !x.Contains("Prop2=31")));
                    }
                }

                if (fileShouldBeDeleted)
                {
                    File.SetAttributes(filePath, FileAttributes.Normal);
                    File.Delete(filePath);
                }
            }
        }

        private static long FormatSizeinKB(long size) => size > 1000 ? size / 1000 : 1;

        private static void ForceWindowsExplorerToShowTag(string folderPath)
        {
            Vanara.PInvoke.Shell32.SHFOLDERCUSTOMSETTINGS settings = new SHFOLDERCUSTOMSETTINGS();
            settings.dwSize = (uint)Marshal.SizeOf(typeof(SHFOLDERCUSTOMSETTINGS));
            settings.dwMask = FOLDERCUSTOMSETTINGSMASK.FCSM_INFOTIP;
            settings.pszInfoTip = "A text folder2.";
            var result = Vanara.PInvoke.Shell32.SHGetSetFolderCustomSettings(ref settings, folderPath, FCS.FCS_FORCEWRITE);
            Console.Write(result);
            return;
        }


    }
}


