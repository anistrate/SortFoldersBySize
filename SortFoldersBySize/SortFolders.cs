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
        private readonly SizeCalculator sizeCalculator;
        private readonly CommandInterpreter interpreter = new CommandInterpreter();
        public SortFolders(IFileSystem fileSystem)
        {
            sizeCalculator = new SizeCalculator(fileSystem);
        }

        static void Main(string[] args)
        {
            var programInstance = new SortFolders(new FileSystem());
            var argResult = programInstance.interpreter.InterpretCommand(args);

            if(argResult.IsFailure)
            {
                //TODO
                //to use windows tray notifications
                //also consider a log file
                Console.WriteLine(argResult.Error);
                return;
            }

            var command = argResult.Value;
            var folderExistsResult = programInstance.sizeCalculator.FolderExists(command.RootPath);

            if(folderExistsResult.IsFailure)
            {
                //TODO
                //to use windows tray notifications
                //also consider a log file
                Console.WriteLine(folderExistsResult.Error);
                return;
            }

            var commandResult = programInstance.sizeCalculator.ExecuteCommand(command);
            
            if(commandResult.IsSuccess)
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


