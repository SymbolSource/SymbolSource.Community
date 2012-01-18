using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuGet;
using NuGet.Commands;

namespace SymbolSource.Integration.NuGet.CommandLine
{
    [Command(typeof(ValidateCommandResources), "validate", "Description")]
    public class ValidateCommand : Command
    {
        public override void ExecuteCommand()
        {
            throw new NotImplementedException();
        }
    }
}
