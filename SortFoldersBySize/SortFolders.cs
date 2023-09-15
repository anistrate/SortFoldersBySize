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

        public readonly ArgumentParser argumentParser = new ArgumentParser();
        public readonly FolderService folderService;
        public readonly FolderTaggingService folderTagService;
        public SortFolders(IFileSystem fileSystem, FolderService _folderService = null, FolderTaggingService _folderTaggingService = null)
        {
            folderService = _folderService ?? new FolderService(fileSystem);
            folderTagService = _folderTaggingService ?? new FolderTaggingService(fileSystem);
            argumentParser = new ArgumentParser();
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
            var folderExistsResult = programInstance.folderService.FolderExists(command.RootPath);

            if (folderExistsResult.IsFailure)
            {
                //TODO
                //to use windows tray notifications
                //also consider a log file
                return;
            }

            var commandResult = programInstance.ExecuteCommand(command);

            if (commandResult.IsSuccess)
            {
                EvilHackToForceWindowsExplorerToRefreshTheTags(command.RootPath);
            }
            else
            {
                //the fuck do we do here??
            }

            //TODO add a -help or --help command??
        }

        private Result ExecuteCommand(CommandArgs command)
        {
            Result result;
            switch (command.Command)
            {
                case CommandConstants.Calculate:
                    var directoriesDictionary = folderService.GetSizesForFoldersAtPath(command.RootPath);
                    result = folderTagService.SetTagsForFolders(directoriesDictionary);
                    break;
                case CommandConstants.RemoveTags:
                    result = folderTagService.RemoveFolderTags(command.RootPath);
                    break;
                default:
                    throw new ArgumentException($"Invalid argument {command.Command}");
            }
            return result;
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


