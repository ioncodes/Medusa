using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Medusa
{
    public class SecretsScanner
    {
        private readonly HashSet<char> base64Characters = new HashSet<char>() {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
            'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f',
            'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v',
            'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '/',
            '='
        };
        private string MD5Regex = "^[a-f0-9]{32}$";
        private string SHA1Regex = "\b([a-f0-9]{40})\b";
        private string UrlRegex = "^http(s)?://([\\w-]+.)+[\\w-]+(/[\\w- ./?%&=])?$";
        private string EmailRegex =
                "^(?(\")(\".+?(?<!\\\\)\"@)|(([0-9a-z]((\\.(?!\\.))|[-!#\\$%&\'\\*\\+/=\\?\\^`\\{\\}\\|~\\w])*)(?<=[0-9a-z])@))(?(\\[)(\\[(\\d{1,3}\\.){3}\\d{1,3}\\])|(([0-9a-z][-\\w]*[0-9a-z]*\\.)+[a-z0-9][\\-a-z0-9]{0,22}[a-z0-9]))$"
            ;
        private OpCode NetworkCredentialsOpCode = OpCodes.Callvirt;
        private string NetworkCredentialsOperand = "System.Net.Mail.SmtpClient::set_Credentials";
        private ModuleDefMD module;

        public SecretsScanner(ModuleDefMD module)
        {
            this.module = module;
        }

        public List<string> Execute()
        {
            var results = new List<string>();
            foreach (var type in module.Types)
            {
                foreach (var method in type.Methods)
                {
                    if (method.Body != null)
                    {
                        foreach (var instruction in method.Body.Instructions)
                        {
                            if (instruction.OpCode == NetworkCredentialsOpCode && instruction.Operand.ToString().ToLower().Contains(NetworkCredentialsOperand.ToLower()))
                            {
                                results.Add("Found network credentials at " + method.FullName);
                            }

                            if (instruction.OpCode == OpCodes.Ldstr)
                            {
                                if(IsBase64String(instruction.Operand.ToString()))
                                {
                                    results.Add("Found Base64 string at " + method.FullName + " -> " + instruction.Operand.ToString());
                                }

                                if(Regex.IsMatch(instruction.Operand.ToString(), MD5Regex))
                                {
                                    results.Add("Found MD5 string at " + method.FullName + " -> " + instruction.Operand.ToString());
                                }

                                if(Regex.IsMatch(instruction.Operand.ToString(), SHA1Regex))
                                {
                                    results.Add("Found SHA1 string at " + method.FullName + " -> " + instruction.Operand.ToString());
                                }

                                if (Regex.IsMatch(instruction.Operand.ToString(), EmailRegex))
                                {
                                    results.Add("Found Email at " + method.FullName + " -> " + instruction.Operand.ToString());
                                }

                                if (Regex.IsMatch(instruction.Operand.ToString(), UrlRegex))
                                {
                                    results.Add("Found Url at " + method.FullName + " -> " + instruction.Operand.ToString());
                                }
                            }
                        }
                    }
                }
            }
            return results;
        }


        private bool IsBase64String(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            else if (value.Any(c => !base64Characters.Contains(c)))
            {
                return false;
            }

            try
            {
                string base64 = Encoding.Unicode.GetString(Convert.FromBase64String(value));
                return Regex.IsMatch(base64, @"^[a-zA-Z0-9_]+$");
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}