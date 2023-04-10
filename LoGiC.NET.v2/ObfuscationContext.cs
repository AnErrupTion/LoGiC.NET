using dnlib.DotNet;

namespace LoGiC.NET.v2;

public sealed class ObfuscationContext
{
    public readonly ModuleDefMD Module;

    public ObfuscationContext(string path)
    {
        Module = ModuleDefMD.Load(path);
    }
}