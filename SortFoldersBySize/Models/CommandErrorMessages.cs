using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortFoldersBySize.Models
{
    public static class CommandErrorMessages
    {
        public static string IncorrectNumberOfArguments = "Expected 2 arguments, found {0}";
        public static string InvalidParameter = "Invalid parameter {0}, usage: program <path> c|r";
    }
}
