# Introduction
LoGiC.NET is a free and open-source .NET obfuscator that uses dnlib for folks who want to see how obfuscation works with more complex protections than Goldfuscator for example.

The executable in the screenshots below was obfuscated with LoGiC.NET version **1.5**.

# Before obfuscation
<img src="https://github.com/AnErrupTion/LoGiC.NET/raw/master/before.PNG">

# After obfuscation
<img src="https://github.com/AnErrupTion/LoGiC.NET/raw/master/after.PNG">

# Dependencies
dnlib v3.3.3 : Restore NuGet packages.<br/>
SharpConfigParser: https://github.com/AnErrupTion/LoGiC.NET/raw/master/SharpConfigParser.dll

# Optional
Configuration file: https://github.com/AnErrupTion/LoGiC.NET/raw/master/config.txt

# Current Features
- Renames methods, parameters, properties, fields and events.
- Adds proxy calls.
- Encrypts strings.
- Encodes ints.
- Adds junk methods.
- Prevents application tampering.
- Adds control flow.
- And more!

# TODO
- Add an anti-emulation.
- Add an anti-debugger.
- Better junk methods protection.
- Better anti-tamper.
- Better string encryption.
