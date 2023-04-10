namespace LoGiC.NET.v2.Obfuscation;

public abstract class BaseObfuscation
{
    public abstract string Name { get; }

    public abstract void Run(ObfuscationContext context);
}