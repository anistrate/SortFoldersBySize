using Microsoft.Toolkit.Uwp.Notifications;
using SortFoldersBySize.Models;
using SortFoldersBySize.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
using static Vanara.PInvoke.Shell32;
//using Vanara;


namespace SortFolderBySize
{

    public class SortFolders
    {
        private readonly CommandExecutor sizeCalculator;
        private readonly ArgumentParser argumentParser = new ArgumentParser();
        public SortFolders(IFileSystem fileSystem)
        {
            sizeCalculator = new CommandExecutor(fileSystem);
        }

        static void Main(string[] args)
        {
            var programInstance = new SortFolders(new FileSystem());
            var argResult = programInstance.argumentParser.ParseArguments(args);

            if (argResult.IsFailure)
            {
                //TODO
                new ToastContentBuilder()
                    .AddArgument("action", "viewConversation")
                    .AddArgument("conversationId", 9813)
                    .AddText("Crapa baa!")
                    .AddText("Pula, Ete pula!")
                    .Show();
                //also consider a log file
                return;
            }

            var command = argResult.Value;
            var folderExistsResult = programInstance.sizeCalculator.FolderExists(command.RootPath);

            if (folderExistsResult.IsFailure)
            {
                //TODO
                //to use windows tray notifications
                //also consider a log file
                return;
            }

            var commandResult = programInstance.sizeCalculator.ExecuteCommand(command);

            if (commandResult.IsSuccess)
            {
                EvilHackToForceWindowsExplorerToRefreshTheTags(command.RootPath);
            }

            //TODO add a -help or --help command??
        }

        private static void EvilHackToForceWindowsExplorerToRefreshTheTags(string folderPath)
        {
            Vanara.PInvoke.Shell32.SHFOLDERCUSTOMSETTINGS settings = new SHFOLDERCUSTOMSETTINGS();
            settings.dwSize = (uint)Marshal.SizeOf(typeof(SHFOLDERCUSTOMSETTINGS));
            settings.dwMask = FOLDERCUSTOMSETTINGSMASK.FCSM_INFOTIP;
            settings.pszInfoTip = " ";
            var result = Vanara.PInvoke.Shell32.SHGetSetFolderCustomSettings(ref settings, folderPath, FCS.FCS_FORCEWRITE);
            Console.Write(result);
            return;
        }


    }
}


