using SortFoldersBySize.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Vanara.Extensions.Reflection;
using static Vanara.PInvoke.Ole32.PROPERTYKEY.System;
using static Vanara.PInvoke.Shell32;

namespace SortFoldersBySize.Services
{
    public  class SizeCalculator
    {
        private IFileSystem _fileSystem;
        private const string DesktopIniFile = "\\desktop.ini";
        private const string FolderTagLine = "Prop5=31,FolderTag";
        private const string FolderTitleLine = "Prop2=31,FolderTitle";
        private const string InvalidPath = "The specified path {0} does not exist or an error has occured trying to access it.";

        private string[] GetDesktopIniLines()
        {
            return new string[]
            {
                "[.ShellClassInfo]",
                "[{F29F85E0-4FF9-1068-AB91-08002B27B3D9}]",
                FolderTagLine,
                FolderTitleLine,
                "theMagicComment"
            };

        }
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

        public Result ExecuteCommand(CommandArgs command)
        {
            Result result;
            switch(command.Command)
            {
                case CommandConstants.Calculate:
                    var directoriesDictionary = CalculateFolderSizes(command.RootPath);
                    result = SetFolderTags(directoriesDictionary);
                    break;
                case CommandConstants.RemoveTags:
                    result = RemoveFolderTags(command.RootPath);
                    break;
                default:
                    throw new ArgumentException($"Invalid argument {command.Command}");
            }
            return result;
        }

        public Result SetFolderTags(Dictionary<string, long> directories)
        {
            foreach(var directory in directories)
            {
                var path = directory.Key + DesktopIniFile;
                var folderTagCase = GetFolderTagCase(path);
                SetFolderTagsByCase(path, directory.Value, folderTagCase);
            }

            return Result.Ok();
        }

        public Result SetFolderTagsByCase(string path, long size, FolderTagCase folderTagCase )
        {
            Result result;
            switch (folderTagCase)
            {
                case FolderTagCase.DesktopIniNotExist:
                    result = CreateNewDesktopIniForFolder(path, size);
                    break;
                case FolderTagCase.DesktopIniCreatedByProgram:
                    result = CreateNewDesktopIniForFolder(path, size);
                    break;

                case FolderTagCase.DesktopIniCreatedBySystem:
                    result = ModifyDesktopIniCreatedbySystem(path, size);
                    break;

                case FolderTagCase.DesktopIniCreatedBySystemModifiedByProgram:
                    result = ModifyDesktopIniCreatedBySystemModifiedByProgram(path, size);
                    break;
                default:
                    throw new ArgumentException($"Invalid argument {folderTagCase}");
            }
            return result;
        }

        public Result ModifyDesktopIniCreatedBySystemModifiedByProgram(string path, long size)
        {
            var desktopIniSystemModifiedContent = _fileSystem.File.ReadAllLines(path);
            var desktopIniSystemOriginalContent = desktopIniSystemModifiedContent.Where(x => x.StartsWith(';')
                                                                                          && x != MagiGStrings.ForAppendedFiles).ToArray();
            _fileSystem.File.WriteAllLines(path, desktopIniSystemOriginalContent);

            var desktopIniNewContent = GetDesktopIniFileContent(size, MagiGStrings.ForAppendedFiles);
            _fileSystem.File.AppendAllLines(path, desktopIniNewContent);

            return Result.Ok();
        }

        public Result CreateNewDesktopIniForFolder(string path, long size)
        {
            var desktopIniContent = GetDesktopIniFileContent(size, MagiGStrings.ForCreatedFiles);
            _fileSystem.File.WriteAllLines(path, desktopIniContent);

            //investigate
            //File.SetAttributes(folder, FileAttributes.ReadOnly);
            return Result.Ok();
        }

        public Result ModifyDesktopIniFileCreatedbyProgram(string path, long newSize)
        {
            var sizeInKb = FormatSizeinKB(newSize);

            var desktopIniContent = _fileSystem.File.ReadAllLines(path + DesktopIniFile);
            desktopIniContent[2] = FolderTagLine.Replace("FolderTag", sizeInKb.ToString());
            desktopIniContent[3] = FolderTitleLine.Replace("FolderTitle", FormatSizeInLargestUnit(sizeInKb));
            _fileSystem.File.WriteAllLines(path + DesktopIniFile, desktopIniContent);

            //investigate
            //File.SetAttributes(folder, FileAttributes.ReadOnly);
            return Result.Ok();
        }

        public Result ModifyDesktopIniCreatedbySystem(string path, long size)
        {
            var desktopIniSystemContent = _fileSystem.File.ReadAllLines(path);
            for(int i =0; i< desktopIniSystemContent.Length; i++)
            {
                desktopIniSystemContent[i] = ';' + desktopIniSystemContent[i];
            }
            _fileSystem.File.WriteAllLines(path, desktopIniSystemContent);

            var desktopIniNewContent = GetDesktopIniFileContent(size , MagiGStrings.ForAppendedFiles);
            _fileSystem.File.AppendAllLines(path, desktopIniNewContent);

            //investigate
            //File.SetAttributes(folder, FileAttributes.ReadOnly);
            return Result.Ok();

        }

        private string[] GetDesktopIniFileContent(long size, string magicComment)
        {
            var sizeInKb = FormatSizeinKB(size);

            var desktopIniLines = GetDesktopIniLines();
            desktopIniLines[2] = desktopIniLines[2].Replace("FolderTag", sizeInKb.ToString());
            desktopIniLines[3] = desktopIniLines[3].Replace("FolderTitle", FormatSizeInLargestUnit(sizeInKb));
            desktopIniLines[4] = magicComment;

            return desktopIniLines;
        }

        private long FormatSizeinKB(long size) => size > 1000 ? size/1000 : 1;

        private string FormatSizeInLargestUnit(long kilobytes)
        {
            double size = kilobytes;
            string[] units = { "KB", "MB", "GB" };

            int unitIndex = 0;
            while (size >= 1024 && unitIndex < units.Length - 1)
            {
                size /= 1024;
                unitIndex++;
            }

            return $"{size:0.##} {units[unitIndex]}";
        }

        public FolderTagCase GetFolderTagCase(string path)
        {
            var desktopIniExists =  _fileSystem.File.Exists(path);
            if (!desktopIniExists) return FolderTagCase.DesktopIniNotExist;

            using var reader = new StreamReader(path);
            var content = reader.ReadToEnd();

            if (content.Contains(MagiGStrings.ForCreatedFiles)) return FolderTagCase.DesktopIniCreatedByProgram;
            if (content.Contains(MagiGStrings.ForAppendedFiles)) return FolderTagCase.DesktopIniCreatedBySystemModifiedByProgram;

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

        public Result RemoveFolderTags(string mainFolderPath)
        {
            string[] directories = _fileSystem.Directory.GetDirectories(mainFolderPath);
            foreach (string directory in directories)
            {
                var path = directory + DesktopIniFile;
                var folderTagCase = GetFolderTagCase(path);
                RemoveDesktopIniByCase(path, folderTagCase);
            }

            return Result.Ok();
        }

        public Result RemoveDesktopIniByCase(string path, FolderTagCase folderTagCase)
        {
            Result result;
            switch (folderTagCase)
            {
                case FolderTagCase.DesktopIniNotExist:
                    //Basically do nothing?
                    result = Result.Ok();
                    break;
                case FolderTagCase.DesktopIniCreatedByProgram:
                    result = RemoveDesktopIniCreatedByProgram(path);
                    break;

                case FolderTagCase.DesktopIniCreatedBySystem:
                    //Basically do nothing?
                    result = Result.Ok();
                    break;
                case FolderTagCase.DesktopIniCreatedBySystemModifiedByProgram:
                    result = CleanDesktopIniFromProgramTagInfo(path);
                    break;
                default:
                    throw new ArgumentException($"Invalid argument {folderTagCase}");
            }
            return result;
        }

        public Result RemoveDesktopIniCreatedByProgram(string path)
        {
            _fileSystem.File.Delete(path);
            return Result.Ok();
        }

        public Result CleanDesktopIniFromProgramTagInfo(string path)
        {
            var desktopIniCurrentContent = _fileSystem.File.ReadAllLines(path);
            var desktopIniOriginalContent = desktopIniCurrentContent.Where(x => x[0] == ';' && x != MagiGStrings.ForAppendedFiles)
                                                                    .Select( x => x.Substring(1,x.Length-1))
                                                                    .ToArray();
            _fileSystem.File.WriteAllLines(path, desktopIniOriginalContent);

            return Result.Ok();

        }

    }
}
