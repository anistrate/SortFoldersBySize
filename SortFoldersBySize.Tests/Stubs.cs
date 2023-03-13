using SortFoldersBySize.Models;

namespace SortFoldersBySize.Tests
{
    public static class Stubs
    {
        private static string DefaultPath = "D:/";
        private static string CalculateCommand = "c";
        private static string RemoveCommand = "r";
        private static string IncorrectCommand_i = "i";
        private static string CalculateCommandUpperCase = "C";


        public static Result<CommandArgs> GetCalculateCommand()
        {
            return Result.Ok<CommandArgs>(new CommandArgs(CommandConstants.Calculate, DefaultPath));
        }

        public static Result<CommandArgs> GetRemoveCommand()
        {
            return Result.Ok<CommandArgs>(new CommandArgs(CommandConstants.RemoveTags, DefaultPath));
        }

        public static Result<CommandArgs> GetIncorrectCommandResult()
        {
            return Result.Fail<CommandArgs>(string.Format(CommandErrorMessages.InvalidParameter, IncorrectCommand_i));
        }

        public static Result<CommandArgs> GetIncorrectNumberOfArgumentsResult(int numberOfArguments)
        {
            return Result.Fail<CommandArgs>(string.Format(CommandErrorMessages.IncorrectNumberOfArguments, numberOfArguments));
        }

        public static string[] GetArgsCalculateCorrect()
        {
            return new string[] { DefaultPath, CalculateCommand };
        }

        public static string[] GetArgsCalculateUppercaseCorrect()
        {
            return new string[] { DefaultPath, CalculateCommandUpperCase };
        }

        public static string[] GetArgsRemoveCorrect()
        {
            return new string[] { DefaultPath, RemoveCommand };
        }

        public static string[] GetArgsIncorrectCommand()
        {
            return new string[] { DefaultPath, IncorrectCommand_i };
        }

        public static string[] GetArgsMoreThan2()
        {
            return new string[] { DefaultPath, IncorrectCommand_i, IncorrectCommand_i };
        }

        public static string[] GetArgsOnlyOne()
        {
            return new string[] { DefaultPath};
        }





    }
}
