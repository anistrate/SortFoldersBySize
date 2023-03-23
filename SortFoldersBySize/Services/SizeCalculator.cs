using SortFoldersBySize.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static Vanara.PInvoke.Ole32.PROPERTYKEY.System;
using static Vanara.PInvoke.Shell32;

namespace SortFoldersBySize.Services
{
    public  class SizeCalculator
    {
        private IFileSystem _fileSystem;
        private const string DesktopIniFile = "/desktop.ini";
        private string[] DesktopIniLines = new string[] { "[.ShellClassInfo]"
                                                                  ,"[{F29F85E0-4FF9-1068-AB91-08002B27B3D9}]"
                                                                  ,"Prop5=31,FolderTag"
                                                                  ,"Prop2=31,Title"
                                                                  ,MagicCommentForCreatedFiles
                                                                  };

        private static string MagicCommentForCreatedFiles = "; DangerCouldBeMyMiddleNameButItsJohn";
        private static string MagicCommentForAppendedFiles = "; WatchMrRobotNoW";


        private const string InvalidPath = "The specified path {0} does not exist or an error has occured trying to access it.";
        public SizeCalculator(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public Result FolderExists(string path)
        {
            if (!_fileSystem.Directory.Exists(path))
            {
                return Result.Fail(string.Format(InvalidPath, path));
            }

            return Result.Ok();
        }

        public Result InterpretCommand(CommandArgs command)
        {
            switch(command.Command)
            {
                case CommandConstants.Calculate:
                    var directoriesDictionary = CalculateFolderSizes(command.RootPath);
                    var result = SetFolderTags(directoriesDictionary);
                    return Result.Ok();
                    break;
                case CommandConstants.RemoveTags:

                    return Result.Ok();
                    break;
                default:
                    throw new ArgumentException($"Invalid argument {command.Command}");
            }


        }

        public Result SetFolderTags(Dictionary<string, long> directories)
        {
            foreach(var directory in directories)
            {
                var folderTagCase = GetFolderTagCase(directory + DesktopIniFile);

                switch(folderTagCase)
                {
                    case FolderTagCase.DesktopIniNotExist:
                        CreateDesktopIniForFolder(directory.Key, directory.Value);
                        break;
                    case FolderTagCase.DesktopIniCreatedByThis:

                        break;

                    case FolderTagCase.DesktopIniCreatedBySystem:

                        break;
                    case FolderTagCase.DesktopIniModifiedByThis:

                        break;
                    default:
                        throw new ArgumentException($"Invalid argument {folderTagCase}");
                }
            }

            return Result.Ok();
        }

        public Result CreateDesktopIniForFolder(string path, long size)
        {
            _fileSystem.File.Create(path + DesktopIniFile);
            var fileLines = 

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

        private string[] GetDesktopIniFileLines(long size)
        {
            DesktopIniLines[2] = DesktopIniLines[2].Replace("FolderTag", FormatSizeinKB(size).ToString());
            return DesktopIniLines;
            // or 
            return DesktopIniLines.Select(x => x.Replace("FolderTag", FormatSizeinKB(size).ToString())).ToArray();
        }

        private static long FormatSizeinKB(long size) => size > 1000 ? size / 1000 : 1;

        public FolderTagCase GetFolderTagCase(string path)
        {
            var desktopIniExists =  _fileSystem.File.Exists(path);
            if (!desktopIniExists) return FolderTagCase.DesktopIniNotExist;

            using var reader = new StreamReader(path);
            var content = reader.ReadToEnd();

            if (content.Contains(MagiGStrings.MagicCommentForCreatedFiles)) return FolderTagCase.DesktopIniCreatedByThis;
            if (content.Contains(MagiGStrings.MagicCommentForModifiedFiles)) return FolderTagCase.DesktopIniModifiedByThis;

            return FolderTagCase.DesktopIniCreatedBySystem;

        }

        public Dictionary<string, long> CalculateFolderSizes(string path)
        {
            var directoriesDictionary = new Dictionary<string, long>();
            string[] directories = _fileSystem.Directory.GetDirectories(path);
            foreach (string directory in directories)
            {
                directoriesDictionary.Add(directory, GetFolderSize(directory));
            }
            return directoriesDictionary;
        }

        public long GetFolderSize(string folderPath)
        {
            long folderSize = 0;
            var dirInfo = _fileSystem.DirectoryInfo.New(folderPath);
            foreach (var fi in dirInfo.GetFiles("*", SearchOption.AllDirectories))
            {
                folderSize += fi.Length;
            }
            return folderSize;
        }

        public void RemoveFolderTags(string path)
        {

        }
    }
}
