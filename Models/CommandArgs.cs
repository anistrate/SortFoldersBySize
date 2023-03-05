using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortFoldersBySize.Models
{
    public class CommandArgs
    {
        public string Command { get; set; }
        public string RootPath { get; set; }

        public CommandArgs(string Command, string RootPath)
        {
            this.Command = Command;
            this.RootPath = RootPath;
        }

    }
}
