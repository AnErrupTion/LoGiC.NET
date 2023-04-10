using LoGiC.NET.v2;
using LoGiC.NET.v2.Obfuscation;

Terminal.Initialize();
Terminal.Info("Loading module");

var path = args[0];
var context = new ObfuscationContext(path);

Terminal.Info("Loading obfuscations");

var obfuscations = new BaseObfuscation[]
{
    new RenameObfuscation()
};

Terminal.Info("Executing obfuscations");

foreach (var obfuscation in obfuscations)
{
    Terminal.Info($"Executing: {obfuscation.Name}");
    obfuscation.Run(context);
}