using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortFoldersBySize.Models
{
    public static class ErrorMessages
    {
        public static string IncorrectNumberOfArguments = "Expected 2 arguments, found {0}";
        public static string InvalidParameter = "Invalid parameter {0}, usage: program <path> c|r";
        public static string InvalidPath = "The specified path {0} does not exist or an error has occured trying to access it.";
    }
}
