using dnlib.DotNet;

namespace LoGiC.NET.v2;

public sealed class ObfuscationContext
{
    public readonly ModuleDefMD Module;

    public Importer Importer;

    public ObfuscationContext(string inputPath)
    {
        Module = ModuleDefMD.Load(inputPath);
        Importer = new Importer(Module);
    }

    public void Save(string outputPath)
    {
        Module.Write(outputPath);
    }
}