using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortFoldersBySize.Models
{
    public static class FolderTaggingHelper
    {
        public const string DesktopIniFile = "\\desktop.ini";
        public const string FolderTagLine = "Prop5=31,FolderTag";
        public const string FolderTitleLine = "Prop2=31,FolderTitle";

        private static string[] GetDesktopIniLines()
        {
            return new string[]
            {
                "[.ShellClassInfo]",
                "[{F29F85E0-4FF9-1068-AB91-08002B27B3D9}]",
                FolderTagLine,
                FolderTitleLine,
                "theMagicComment"
            };

        }

        public static string[] GetDesktopIniFileContent(long size, string magicComment)
        {
            var sizeInKb = FormatSizeinKB(size);

            var desktopIniLines = GetDesktopIniLines();
            desktopIniLines[2] = desktopIniLines[2].Replace("FolderTag", sizeInKb.ToString());
            desktopIniLines[3] = desktopIniLines[3].Replace("FolderTitle", FormatSizeInLargestUnit(sizeInKb));
            desktopIniLines[4] = magicComment;

            return desktopIniLines;
        }

        public static string FormatSizeInLargestUnit(long kilobytes)
        {
            double size = kilobytes;
            string[] units = { "KB", "MB", "GB" };

            int unitIndex = 0;
            while (size >= 1024 && unitIndex < units.Length - 1)
            {
                size /= 1024;
                unitIndex++;
            }

            return $"{size:0.##} {units[unitIndex]}";
        }

        public static long FormatSizeinKB(long size) => size > 1000 ? size / 1000 : 1;
    }
}
