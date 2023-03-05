using SortFoldersBySize.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortFoldersBySize.Services
{
    public class CommandInterpreter
    {

        public const string IncorrectNumberOfArguments = "Expected 2 arguments, found {0}";

        public Result<CommandArgs> InterpretCommand(string[] args)
        {
            if(args.Length !=2)
            {
                return Result.Fail<CommandArgs>(String.Format(IncorrectNumberOfArguments, args.Length) );
            }


            if (!Directory.Exists(args[0]))
            {

            }

            if (args[1]=="C")
            {

            }

            if (args[1] == "r") 
            {

            }

            
        }

    }
}
