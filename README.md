# Introduction
LoGiC.NET is a free and open-source .NET obfuscator that uses dnlib for folks that want to see how obfuscation works with more complex obfuscations than Goldfuscator for example.

# Before obfuscation
<img src="https://i.imgur.com/0J5ZDq0.png">

# After obfuscation
<img src="https://i.imgur.com/W68kj01.png">

# Dependencies
dnlib v3.3.1 : Restore NuGet packages and it'll work.<br/>
SharpConfigParser : https://mega.nz/#!c3BxUKoK!3Uvx6izl_Gv1hnGJOzeBSRs4EzcaIjCCOMA2SgKW5FM

# Current Features
- Renames methods, parameters, properties, fields and events.
- Adds proxy calls.
- Encrypts strings.
- Encodes ints.
- Adds junk methods.
- Prevents application tampering.

# TODO
- Add an Anti-Emulation and Anti-Debug.
