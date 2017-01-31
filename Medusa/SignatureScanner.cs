using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Medusa
{
    public class SignatureScanner
    {
        private ModuleDefMD module;
        private OpCode[] opCodes;

        public SignatureScanner(ModuleDefMD module, OpCode[] opCodes)
        {
            this.module = module;
            this.opCodes = opCodes;
        }

        public List<string> Execute()
        {
            List<string> results = new List<string>();
            foreach (var type in module.Types)
            {
                foreach (var method in type.Methods)
                {
                    if (method.Body != null)
                    {
                        List<OpCode> tempList = opCodes.ToList();
                        foreach (var instruction in method.Body.Instructions)
                        {
                            if (tempList.Contains(instruction.OpCode))
                            {
                                results.Add(method.FullName);
                            }
                        }
                    }
                }
            }
            return results;
        }
    }
}