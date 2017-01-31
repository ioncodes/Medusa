using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;

namespace Medusa
{
    public class ObfuscatorScanner
    {
        private Dictionary<string, string> attributes;

        private string UnknownRegex = "";

        private ModuleDefMD module;

        public ObfuscatorScanner(ModuleDefMD module, Dictionary<string,string> attributes)
        {
            this.module = module;
            this.attributes = attributes;
        }

        public List<string> Execute()
        {
            var results = new List<string>();
            var entropies = new List<double>();
            foreach (var type in module.Types)
            {
                foreach (var obfuscator in attributes)
                {
                    if (type.FullName.Contains(obfuscator.Value))
                    {
                        results.Add("Found obfuscator: " + obfuscator.Key);
                    }
                    entropies.Add(ShannonEntropy(type.FullName));
                }
            }
            if (results.Count == 0)
            {
                double entropy = entropies.Average();
                if ((entropy > 1.5 && entropy < 2) || entropy > 4 && entropy < 5)
                {
                    results.Add("Possible unknown obfuscator");
                }
            }
            return results.Distinct().ToList();
        }

        private double ShannonEntropy(string s)
        {
            var map = new Dictionary<char, int>();
            foreach (char c in s)
            {
                if (!map.ContainsKey(c))
                    map.Add(c, 1);
                else
                    map[c] += 1;
            }

            int len = s.Length;

            return map.Select(item => (double) item.Value / len).Aggregate(0.0, (current, frequency) => current - frequency * (Math.Log(frequency) / Math.Log(2)));
        }
    }
}