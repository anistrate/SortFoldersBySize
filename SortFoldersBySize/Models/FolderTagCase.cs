using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortFoldersBySize.Models
{
    public enum FolderTagCase
    {
        DesktopIniNotExist = 0,
        DesktopIniCreatedBySystem,
        DesktopIniCreatedByProgram,
        DesktopIniCreatedBySystemModifiedByProgram
    }
}
