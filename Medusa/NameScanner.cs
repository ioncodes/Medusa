using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;

namespace Medusa
{
    internal class NameScanner
    {
        private string[] stringList = {"License","Password","Key","DRM","Obfuscator","Obfuscation","Hack","Crack"};
        private ModuleDefMD module;

        public NameScanner(ModuleDefMD module)
        {
            this.module = module;
        }

        public NameScanner(ModuleDefMD module, string[] stringList)
        {
            this.module = module;
            this.stringList = stringList;
            this.stringList = stringList.Select(s => s.ToLowerInvariant()).ToArray();
        }

        public List<string> Execute()
        {
            var results = new List<string>();
            foreach (var type in module.Types)
            {
                if(stringList.Any(type.Name.String.ToLower().Contains))
                    results.Add("Found name: " + type.FullName);
                results.AddRange(from method in type.Methods where stringList.Any(method.Name.String.ToLower().Contains) select method.FullName);
            }
            return results;
        }
    }
}