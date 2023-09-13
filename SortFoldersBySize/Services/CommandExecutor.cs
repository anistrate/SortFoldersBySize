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
    public  class CommandExecutor
    {
        private IFileSystem _fileSystem;
        private FolderSizeCalculator _folderSizeCalculator;
        private readonly FolderTagService _folderTagService;

        public CommandExecutor(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _folderSizeCalculator = new FolderSizeCalculator(fileSystem);

        }

        public Result FolderExists(string path)
        {
            if (!_fileSystem.Directory.Exists(path))
            {
                return Result.Fail(string.Format(ErrorMessages.InvalidPath, path));
            }

            return Result.Ok();
        }

        public Result ExecuteCommand(CommandArgs command)
        {
            Result result;
            switch(command.Command)
            {
                case CommandConstants.Calculate:
                    var directoriesDictionary = _folderSizeCalculator.CalculateFolderSizes(command.RootPath);
                    result = _folderTagService.SetTagsForFolders(directoriesDictionary);
                    break;
                case CommandConstants.RemoveTags:
                    result = _folderTagService.RemoveFolderTags(command.RootPath);
                    break;
                default:
                    throw new ArgumentException($"Invalid argument {command.Command}");
            }
            return result;
        }


    }
}
