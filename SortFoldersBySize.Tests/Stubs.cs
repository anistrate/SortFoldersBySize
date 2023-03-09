using SortFoldersBySize.Models;

namespace SortFoldersBySize.Tests
{
    public static class Stubs
    {
        private static string DefaultPath = "D:/";
        private static string CalculateCommand1 = "c";
        private static string RemoveCommand1 = "r";
        private static string IncorrectCommand_i = "i";

        private static string IncorretArgumentErrorMessage = "Invalid parameter {0}, usage: program <path> c|r";

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
            return Result.Fail<CommandArgs>(string.Format(IncorretArgumentErrorMessage, IncorrectCommand_i));
        }

        public static string[] GetArgsCalculateCorrect()
        {
            return new string[] { DefaultPath, CalculateCommand1 };
        }

        public static string[] GetArgsRemoveCorrect()
        {
            return new string[] { DefaultPath, RemoveCommand1 };
        }

        public static string[] GetArgsIncorrectCommand()
        {
            return new string[] { DefaultPath, IncorrectCommand_i };
        }





    }
}
