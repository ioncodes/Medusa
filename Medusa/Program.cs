using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Medusa
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Medusa.exe [filename]");
                Console.Read();
                Environment.Exit(0);
            }
            var file = args[0];

            var module = ModuleDefMD.Load(file);

            var stringScanner = new StringScanner(module);
            var strings = stringScanner.Execute();
            foreach (var s in strings)
            {
                Console.WriteLine(s);
            }

            var sigScanner = new SignatureScanner(module, new []
            {
                OpCodes.Ldstr
            });
            var locs = sigScanner.Execute();
            foreach (var l in locs)
            {
                Console.WriteLine(l);
            }

            var obfScanner = new ObfuscatorScanner(module);
            var obfs = obfScanner.Execute();
            foreach (var o in obfs)
            {
                Console.WriteLine(o);
            }

            var nameScanner = new NameScanner(module);
            var names = nameScanner.Execute();
            foreach (var name in names)
            {
                Console.WriteLine(name);
            }

            var secretsScanner = new SecretsScanner(module);
            var secrets = secretsScanner.Execute();
            foreach (var secret in secrets)
            {
                Console.WriteLine(secret);
            }
            Console.Read();
        }
    }
}