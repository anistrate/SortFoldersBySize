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
        
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (Object.ReferenceEquals(this, obj)) return true;
            var cmd = (CommandArgs)obj;
            return this.Command == cmd.Command
                && this.RootPath == cmd.RootPath;
        }

    }
}
