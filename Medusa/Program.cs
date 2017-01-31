using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

            Helpers helpers = new Helpers();

            var module = ModuleDefMD.Load(file);

            var stringScanner = new StringScanner(module, File.ReadAllLines("strings.txt"), StringScanner.SearchModes.CaseInsensitive | StringScanner.SearchModes.Contains);
            var strings = stringScanner.Execute();
            foreach (var s in strings)
            {
                Console.WriteLine(s);
            }

            string[] sigCodes = File.ReadAllLines("signature.txt");
            if (sigCodes.Length != 0)
            {
                List<OpCode> opcodes = new List<OpCode>();
                foreach (var sigCode in sigCodes)
                {
                    opcodes.Add(helpers.GetField(sigCode));
                }
                var sigScanner = new SignatureScanner(module, opcodes.ToArray());
                var locs = sigScanner.Execute();
                foreach (var l in locs)
                {
                    Console.WriteLine(l);
                }
            }

            string[] obfus = File.ReadAllLines("obfuscators.txt");
            Dictionary<string,string> dicObf = obfus.ToDictionary(o => o.Split(':')[0], o => o.Split(':')[1]);
            var obfScanner = new ObfuscatorScanner(module, dicObf);
            var obfs = obfScanner.Execute();
            foreach (var o in obfs)
            {
                Console.WriteLine(o);
            }

            string[] ns = File.ReadAllLines("names.txt");
            var nameScanner = new NameScanner(module, ns);
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


        class Helpers
        {
            public OpCode GetField(string sigCode)
            {
                var type = typeof(OpCodes);
                var field = type.GetField(sigCode, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Static);
                return (OpCode) field.GetValue(this);
            }
        }
    }
}