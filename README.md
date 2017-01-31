# Medusa
Medusa - .NET Multi-Scanner [BETA]

## What is Medusa?
Medusa is a scanner for .NET assemblies. It scans for obfuscators, etc.

* Scans for obfuscators
* Scans for interesting method/type names
* Scans for interesting strings in methods
* Scans for signatures in methods (via opcodes)
* Scans for secrets in methodbodies such as the NetworkCredential object or MD5 hashes, emails, Base64 encrypted strings, etc

## How to use
Download and compile. I built it with Jetbrains Raider, but it should work with VS too. Copy & Paste the *.txt files from the 'Configs' folder to the binary directory.
Drag & Drop the target assembly onto Medusa.exe or open cmd and enter "Medusa.exe [target]".

## Configs
* strings.txt -> on each line a string which should be looked for (it's case insensitive and it checks via 'Contains()')
* names.txt -> same as strings.txt but for the type & method names.
* obfuscators.txt -> a list which looks like this: [Name]:[Identifier]; Name should be the name of the obfuscator and Identifier the identifier which the obfuscator writes to the assembly in form of a type.
Example: "ConfuserEx:ConfusedByAttribute"
* signature.txt -> if empty, it wont check signatures. Enter OpCodes on each line. It's case sensitive (I think so), and must look like the fields in the OpCodes class in dnlib.

## Screenies
I don't have a cool example so I will let it check itself as of now.

![Screen](http://i.imgur.com/7ZNz2zS.png)
