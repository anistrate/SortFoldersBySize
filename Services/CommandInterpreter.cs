using SortFoldersBySize.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Abstractions;

namespace SortFoldersBySize.Services
{
    public class CommandInterpreter
    {

        private const string IncorrectNumberOfArguments = "Expected 2 arguments, found {0}";
        private const string FilePathNotFound = "Filepath {0} not found";
        private const string InvalidParameter = "Invalid parameter {0}, usage: program <path> c|r";

        private IFileSystem _fileSystem;

        public CommandInterpreter(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public Result<CommandArgs> InterpretCommand(string[] args)
        {
            if(args.Length != 2)
            {
                return Result.Fail<CommandArgs>(string.Format(IncorrectNumberOfArguments, args.Length) );
            }

            if (!_fileSystem.Directory.Exists(args[0]))
            {
                return Result.Fail<CommandArgs>(string.Format(FilePathNotFound, args[0]));
            }

            if (args[1]=="c" || args[1] == "C")
            {
                return Result.Ok(new CommandArgs(CommandConstants.Calculate, args[0]));
            }

            if (args[1] == "r" || args[1] == "R") 
            {
                return Result.Ok(new CommandArgs(CommandConstants.RemoveTags, args[0]));
            }

            return Result.Fail<CommandArgs>(string.Format(InvalidParameter, args[1]));
            
        }

    }
}
