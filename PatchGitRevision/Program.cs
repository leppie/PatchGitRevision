using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Mono.Cecil;
using System.Reflection;
using System.IO;
using Mono.Cecil.Cil;

namespace PatchGitRevision
{
  class Program
  {
    static int Main(string[] args)
    {
      string line = null;
      while ((line = Console.ReadLine()) != null)
      {
        var cs = line;

        var ass = AssemblyDefinition.ReadAssembly("IronScheme.dll");
        var type = ass.MainModule.GetType("IronScheme.Hosting", "IronSchemeConsoleHost");
        var f = type.Fields.SingleOrDefault(x => x.Name == "VERSION");
        f.Constant = "Git:" + cs;

        var cctor = type.Methods.SingleOrDefault(x => x.Name == ".cctor");
        cctor.Body.Instructions[0] = Instruction.Create(OpCodes.Ldstr, "Git:" + cs);

        StrongNameKeyPair sn = new StrongNameKeyPair(File.OpenRead("DEVELOPMENT.snk"));

        ass.Write("IronScheme.dll",
          new WriterParameters
          {
            StrongNameKeyPair = sn,
          });

        return 0;
      }

      return 1;
    }
  }
}
