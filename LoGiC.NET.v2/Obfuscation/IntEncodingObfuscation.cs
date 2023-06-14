using dnlib.DotNet.Emit;
using LoGiC.NET.v2.Utils;

namespace LoGiC.NET.v2.Obfuscation;

public sealed class IntEncodingObfuscation : BaseObfuscation
{
    public override string Name => "Int encoding";

    private uint _encodedInts;

    public override void Run(ObfuscationContext context)
    {
        var absMethod = context.Module.Import(typeof(Math).GetMethod("Abs", new[] { typeof(int) }));
        var minMethod = context.Module.Import(typeof(Math).GetMethod("Min", new[] { typeof(int), typeof(int) }));
        var maxMethod = context.Module.Import(typeof(Math).GetMethod("Max", new[] { typeof(int), typeof(int) }));

        foreach (var type in context.Module.Types)
        {
            foreach (var method in type.Methods)
            {
                if (!method.HasBody)
                {
                    Terminal.Warn($"Method has no body: {method.FullName}");
                    continue;
                }

                var instructions = new List<Instruction>(method.Body.Instructions);
                
                for (var i = 0; i < instructions.Count; i++)
                {
                    var instruction = instructions[i];

                    if (!instruction.IsLdcI4())
                    {
                        continue;
                    }

                    var operand = instruction.GetLdcI4Value();

                    if (operand >= 0)
                    {
                        method.Body.Instructions.Insert(i + 1, OpCodes.Call.ToInstruction(absMethod));
                    }

                    var neg = NumberUtils.Random.Next(2, 65).GetHashCode();
                    if (neg % 2 != 0)
                    {
                        neg++;
                    }

                    for (var j = 0; j < neg; j++)
                    {
                        method.Body.Instructions.Insert(i + j + 1, OpCodes.Neg.ToInstruction());
                    }

                    if (operand > 1)
                    {
                        method.Body.Instructions.Insert(i + 1, OpCodes.Ldc_I4.ToInstruction(1));
                        method.Body.Instructions.Insert(i + 2, OpCodes.Call.ToInstruction(maxMethod));
                    }

                    if (operand < int.MaxValue)
                    {
                        method.Body.Instructions.Insert(i + 1, OpCodes.Ldc_I4.ToInstruction(int.MaxValue));
                        method.Body.Instructions.Insert(i + 2, OpCodes.Call.ToInstruction(minMethod));
                    }
                }

                method.Body.SimplifyBranches();
                method.Body.OptimizeBranches();

                _encodedInts++;
            }
        }

        Terminal.Info($"Encoded {_encodedInts} ints");
    }
}