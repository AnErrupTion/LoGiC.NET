# Introduction
LoGiC.NET is a free and open-source .NET obfuscator that uses dnlib for folks that want to see how obfuscation works with more complex obfuscations than Goldfuscator for example.

# Before obfuscation
<img src="https://i.imgur.com/0J5ZDq0.png">

# After obfuscation
<img src="https://i.imgur.com/W68kj01.png">

# Dependencies
dnlib v3.3.2 : Included with the project, if it doesn't work try to restore NuGet packages.<br/>
SharpConfigParser : https://github.com/AnErrupTion/LoGiC.NET/raw/master/SharpConfigParser.dll

# Current Features
- Renames methods, parameters, properties, fields and events.
- Adds proxy calls.
- Encrypts strings.
- Encodes ints.
- Adds junk methods.
- Prevents application tampering.

# TODO
- Add an Anti-Emulation and Anti-Debug.
