using SortFoldersBySize.Models;

namespace SortFoldersBySize.Tests
{
    public static class Stubs
    {
        private static string DefaultPath = "D:/";

        public static Result<CommandArgs> GetCommand(string command)
        {
            return Result.Ok(new CommandArgs(command, DefaultPath));
        }

        public static Result<CommandArgs> GetIncorrectCommandResult(string incorrectCommand)
        {
            return Result.Fail<CommandArgs>(string.Format(ErrorMessages.InvalidParameter, incorrectCommand));
        }

        public static Result<CommandArgs> GetIncorrectNumberOfArgumentsResult(int numberOfArguments)
        {
            return Result.Fail<CommandArgs>(string.Format(ErrorMessages.IncorrectNumberOfArguments, numberOfArguments));
        }

        public static string[] GetArgsCalculateCorrect(string calculateCommand)
        {
            return new string[] { DefaultPath, calculateCommand };
        }

        public static string[] GetArgsIncorrectCommand(string incorrectCommand)
        {
            return new string[] { DefaultPath, incorrectCommand };
        }

        public static string[] GetArgsMoreThan2(string incorrectCommand1, string incorrectCommand2)
        {
            return new string[] { DefaultPath, incorrectCommand1, incorrectCommand2 };
        }

        public static string[] GetArgsOnlyOne()
        {
            return new string[] { DefaultPath};
        }

    }
}
