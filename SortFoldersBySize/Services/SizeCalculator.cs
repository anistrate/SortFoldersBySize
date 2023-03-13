using SortFoldersBySize.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortFoldersBySize.Services
{
    public  class SizeCalculator
    {
        private IFileSystem _fileSystem;
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


                    return Result.Ok();
                    break;
                case CommandConstants.RemoveTags:

                    return Result.Ok();
                    break;
                default:
                    throw new Exception("Invalid command");
            }


        }

        public void CalculateFolderSizes(string path)
        {

        }

        public void RemoveFolderTags(string path)
        {

        }
    }
}
