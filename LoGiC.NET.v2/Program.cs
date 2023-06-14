using LoGiC.NET.v2;
using LoGiC.NET.v2.Obfuscation;

Terminal.Initialize();

if (args.Length != 2)
{
    Terminal.Error("Correct usage: LoGiC.NET <input.dll> <output.dll>");
    Environment.Exit(1);
    return;
}

Terminal.Info("Loading module");

var inputPath = args[0];
var outputPath = args[1];

var context = new ObfuscationContext(inputPath);

Terminal.Info("Loading obfuscations");

var obfuscations = new BaseObfuscation[]
{
    new RenameObfuscation(),
    new InstructionExpansionObfuscation(),
    //new CallifyInstructionsObfuscation(),
    new IntEncodingObfuscation(),
    //new MethodProxying()
};

Terminal.Info("Executing obfuscations");

foreach (var obfuscation in obfuscations)
{
    Terminal.Info($"Executing: {obfuscation.Name}");
    obfuscation.Run(context);
}

Terminal.Info("Saving file");

context.Save(outputPath);