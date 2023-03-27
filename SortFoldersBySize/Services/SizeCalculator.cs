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
        private const string FolderTagLine = "Prop5=31,FolderTag";
        private string[] DesktopIniLines = new string[] { "[.ShellClassInfo]"
                                                                  ,"[{F29F85E0-4FF9-1068-AB91-08002B27B3D9}]"
                                                                  , FolderTagLine
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
                        var resultCreate = CreateDesktopIniForFolder(directory.Key, directory.Value);
                        break;
                    case FolderTagCase.DesktopIniCreatedByThis:
                        var resultModify = ModifyDesktopIniFileCreatedbyThis(directory.Key, directory.Value);
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
            var desktopIniContent = GetDesktopIniFileContent(size);
            _fileSystem.File.WriteAllLines(path + DesktopIniFile, desktopIniContent);


            //investigate
            //File.SetAttributes(folder, FileAttributes.ReadOnly);
            return Result.Ok();
        }

        public Result ModifyDesktopIniFileCreatedbyThis(string path, long newSize)
        {
            var desktopIniContent = _fileSystem.File.ReadAllLines(path + DesktopIniFile);
            desktopIniContent[2] = FolderTagLine.Replace("FolderTag", FormatSizeinKB(newSize));
            _fileSystem.File.WriteAllLines(path + DesktopIniFile, desktopIniContent);

            //investigate
            //File.SetAttributes(folder, FileAttributes.ReadOnly);
            return Result.Ok();

        }

        private string[] GetDesktopIniFileContent(long size)
        {
            DesktopIniLines[2] = DesktopIniLines[2].Replace("FolderTag", FormatSizeinKB(size));
            //stream.WriteLine(Line4.Replace("Title", FormatSizeinKB(size).ToString("N0") + " KB"));
            return DesktopIniLines;
        }

        private string FormatSizeinKB(long size) => size > 1000 ? (size/1000).ToString() : "1";

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
