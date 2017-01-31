using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.Threading;

namespace Medusa
{
    internal class StringScanner
    {
        private string[] stringList = {"License","Password","Key","DRM","Obfuscator","Obfuscation","Hack","Crack"};
        [Flags]
        public enum SearchModes
        {
            CaseSensitive,
            CaseInsensitive,
            Contains,
            MatchWholeWord
        }
        private SearchModes searchMode = SearchModes.CaseInsensitive | SearchModes.Contains;
        private ModuleDefMD module;

        public StringScanner(ModuleDefMD module)
        {
            this.module = module;
        }

        public StringScanner(ModuleDefMD module, string[] stringList)
        {
            this.module = module;
            this.stringList = stringList;
        }

        public StringScanner(ModuleDefMD module, SearchModes searchMode)
        {
            this.module = module;
            this.searchMode = searchMode;
        }

        public StringScanner(ModuleDefMD module, string[] stringList, SearchModes searchMode)
        {
            this.module = module;
            this.stringList = stringList;
            this.searchMode = searchMode;
            if (searchMode == SearchModes.CaseInsensitive)
            {
                stringList = stringList.Select(s => s.ToLowerInvariant()).ToArray();
            }
        }

        public List<string> Execute()
        {
            var results = new List<string>();
            foreach (var type in module.Types)
            {
                foreach (var method in type.Methods)
                {
                    if (method.Body == null) continue;
                    foreach (var instruction in method.Body.Instructions)
                    {
                        if (instruction.OpCode != OpCodes.Ldstr || instruction.Operand == null) continue;
                        string operand = instruction.Operand.ToString();
                        if (searchMode == SearchModes.CaseInsensitive)
                            operand = operand.ToLower();
                        foreach (var s in stringList)
                        {
                            if (searchMode == SearchModes.Contains && s.Contains(operand))
                            {
                                results.Add("Found string at " + method.FullName + " -> " + operand);
                            }
                            else if (searchMode == SearchModes.MatchWholeWord && s == operand)
                            {
                                results.Add("Found string at " + method.FullName + " -> " +  operand);
                            }
                        }
                    }
                }
            }
            return results;
        }
    }
}