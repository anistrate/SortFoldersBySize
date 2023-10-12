using SortFoldersBySize.Models;
using System.IO.Abstractions;

namespace SortFoldersBySize.Services
{
    public class FolderTaggingService
    {
        private readonly IFileSystem _fileSystem;

        public FolderTaggingService(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public Result SetTagsForFolders(Dictionary<string, long> directories)
        {
            foreach (var directory in directories)
            {
                var path = directory.Key + FolderTaggingHelper.DesktopIniFile;
                var folderTagCase = GetFolderTagCase(path);
                SetFolderTagsByCase(path, directory.Value, folderTagCase);
            }

            return Result.Ok();
        }

        public Result SetFolderTagsByCase(string path, long size, FolderTagCase folderTagCase)
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

            var desktopIniNewContent = FolderTaggingHelper.GetDesktopIniFileContent(size, MagiGStrings.ForAppendedFiles);
            _fileSystem.File.AppendAllLines(path, desktopIniNewContent);

            return Result.Ok();
        }

        public Result CreateNewDesktopIniForFolder(string path, long size)
        {
            try
            {
                var desktopIniContent = FolderTaggingHelper.GetDesktopIniFileContent(size, MagiGStrings.ForCreatedFiles);
                _fileSystem.File.WriteAllLines(path, desktopIniContent);
                return Result.Ok();
            }
            catch(DirectoryNotFoundException dnfe)
            {
                return Result.Fail(dnfe.Message);
            }
            catch(UnauthorizedAccessException uae)
            {
                return Result.Fail(uae.Message);
            }

            //investigate
            //File.SetAttributes(folder, FileAttributes.ReadOnly);
        }

        public Result ModifyDesktopIniCreatedbySystem(string path, long size)
        {
            try
            {
                var desktopIniSystemContent = _fileSystem.File.ReadAllLines(path);
                for (int i = 0; i < desktopIniSystemContent.Length; i++)
                {
                    desktopIniSystemContent[i] = ';' + desktopIniSystemContent[i];
                }
                _fileSystem.File.WriteAllLines(path, desktopIniSystemContent);

                var desktopIniNewContent = FolderTaggingHelper.GetDesktopIniFileContent(size, MagiGStrings.ForAppendedFiles);
                _fileSystem.File.AppendAllLines(path, desktopIniNewContent);

                return Result.Ok();
            }
            catch (DirectoryNotFoundException dnfe)
            {
                return Result.Fail(dnfe.Message);
            }
            catch (UnauthorizedAccessException uae)
            {
                return Result.Fail(uae.Message);
            }

            //investigate
            //File.SetAttributes(folder, FileAttributes.ReadOnly);
        }

        public Result ModifyDesktopIniFileCreatedbyProgram(string path, long newSize)
        {
            var sizeInKb = FolderTaggingHelper.FormatSizeinKB(newSize);

            var desktopIniContent = _fileSystem.File.ReadAllLines(path + FolderTaggingHelper.DesktopIniFile);
            desktopIniContent[2] = FolderTaggingHelper.FolderTagLine.Replace("FolderTag", sizeInKb.ToString());
            desktopIniContent[3] = FolderTaggingHelper.FolderTitleLine.Replace("FolderTitle", FolderTaggingHelper.FormatSizeInLargestUnit(sizeInKb));
            _fileSystem.File.WriteAllLines(path + FolderTaggingHelper.DesktopIniFile, desktopIniContent);

            //investigate
            //File.SetAttributes(folder, FileAttributes.ReadOnly);
            return Result.Ok();
        }

        public FolderTagCase GetFolderTagCase(string path)
        {
            var desktopIniExists = _fileSystem.File.Exists(path);
            if (!desktopIniExists) return FolderTagCase.DesktopIniNotExist;

            using var reader = new StreamReader(path);
            var content = reader.ReadToEnd();

            if (content.Contains(MagiGStrings.ForCreatedFiles)) return FolderTagCase.DesktopIniCreatedByProgram;
            if (content.Contains(MagiGStrings.ForAppendedFiles)) return FolderTagCase.DesktopIniCreatedBySystemModifiedByProgram;

            return FolderTagCase.DesktopIniCreatedBySystem;

        }

        public Result RemoveFolderTags(string mainFolderPath)
        {
            string[] directories = _fileSystem.Directory.GetDirectories(mainFolderPath);
            foreach (string directory in directories)
            {
                var path = directory + FolderTaggingHelper.DesktopIniFile;
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
                                                                    .Select(x => x.Substring(1, x.Length - 1))
                                                                    .ToArray();
            _fileSystem.File.WriteAllLines(path, desktopIniOriginalContent);

            return Result.Ok();

        }
    }
}
