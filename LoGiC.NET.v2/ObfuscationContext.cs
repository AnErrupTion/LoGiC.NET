using dnlib.DotNet;

namespace LoGiC.NET.v2;

public sealed class ObfuscationContext
{
    public readonly ModuleDefMD Module;

    public ObfuscationContext(string inputPath)
    {
        Module = ModuleDefMD.Load(inputPath);
    }

    public void Save(string outputPath)
    {
        Module.Write(outputPath);
    }
}