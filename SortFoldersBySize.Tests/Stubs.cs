using SortFoldersBySize.Models;
using System.IO.Abstractions.TestingHelpers;
using System.Text;

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
            return new string[] { DefaultPath };
        }

        public static MockFileData GetMockDesktopIniFile(long size, string magicComment)
        {
            var desktopIniContent = FolderTaggingHelper.GetDesktopIniFileContent(size, magicComment);
            var stringBuilder = new StringBuilder();
            for (int i = 0; i < desktopIniContent.Length; i++)
            {
                stringBuilder.AppendLine(desktopIniContent[i]);
            }
            return new MockFileData(stringBuilder.ToString());
        }

        public static MockFileData GetMockDesktopIniFileCreatedBySystem()
        {
            var desktopIniContent = FolderTaggingHelper.GetDesktopIniLines();
            var stringBuilder = new StringBuilder();
            for (int i = 0; i < desktopIniContent.Length - 1; i++)
            {
                stringBuilder.AppendLine(desktopIniContent[i]);
            }
            return new MockFileData(stringBuilder.ToString());
        }

        public static string GetMockDesktopIniCreatedBySystemAfterBeingModified(string kbValue, string largestUnitvalue, string unit)
        {
            return ";[.ShellClassInfo]\r\n;[{F29F85E0-4FF9-1068-AB91-08002B27B3D9}]\r\n;Prop5=31,FolderTag\r\n;Prop2=31,FolderTitle\r\n[.ShellClassInfo]\r\n[{F29F85E0-4FF9-1068-AB91-08002B27B3D9}]\r\nProp5=31," + kbValue + "\r\nProp2=31," + largestUnitvalue + " " + unit + "\r\n; WatchMrRobotNoW\r\n";
        }
        //public static string GetMockDesktopIniCreatedBySystemAfterBeingModified(string value, string unit)
        //{
        //    return ";[.ShellClassInfo]\r\n;[{F29F85E0-4FF9-1068-AB91-08002B27B3D9}]\r\n;Prop5=31,FolderTag\r\n;Prop2=31,FolderTitle\\r\\n[.ShellClassInfo]\\r\\n[{F29F85E0-4FF9-1068-AB91-08002B27B3D9}]\\r\\nProp5=31,\"+value+\"\\r\\nProp2=31,\"+ value + \" \" + unit +\"\\r\\n; WatchMrRobotNoW\\r\\n\";";
        //}


    }
}
