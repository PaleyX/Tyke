using System;
using System.Collections.Generic;
using System.Linq;

namespace Tyke.Net.Symbols;

internal static class Linker
{
    internal static bool RunLinker(Process.Process mainProcess)
    {
        Console.WriteLine("Running linker...");

        if (mainProcess == null)
        {
            Errors.Error.ReportError("No main process section found");
            return false;
        }

        int errors = 0;

        // get all linkable commands accross all procedures
        var linkables = new List<ILinkable>();

        // from main process
        linkables.AddRange(mainProcess.GetCommands().Where(c => c is ILinkable).Cast<ILinkable>());

        // from procedures
        foreach(var procedure in SymbolTable.GetProcedures())
        {
            linkables.AddRange(procedure.GetCommands().Where(c => c is ILinkable).Cast<ILinkable>());
        }

        // from sections
        linkables.AddRange(SymbolTable.GetSections().Where(s => s is ILinkable).Cast<ILinkable>());
  
        // do the linking
        foreach (var linkable in linkables)
        {
            foreach (var procedure in SymbolTable.GetProcedures())
            {
                linkable.ProposeProcedure(procedure);
            }

            errors += linkable.LinkComplete();
        }

        Console.WriteLine("Linker completed");
        Console.WriteLine("{0} error(s) found", errors);

        return errors == 0;
    }
}