using dnlib.DotNet.Emit;
using LoGiC.NET.v2.Utils;

namespace LoGiC.NET.v2.Obfuscation;

// TODO: Also encode floats and doubles
public sealed class IntEncodingObfuscation : BaseObfuscation
{
    public override string Name => "Int encoding";

    private uint _encodedInts;

    public override void Run(ObfuscationContext context)
    {
        var absMethod = context.Importer.Import(typeof(Math).GetMethod("Abs", new[] { typeof(int) }));
        var minMethod = context.Importer.Import(typeof(Math).GetMethod("Min", new[] { typeof(int), typeof(int) }));
        var maxMethod = context.Importer.Import(typeof(Math).GetMethod("Max", new[] { typeof(int), typeof(int) }));

        var absLongMethod = context.Importer.Import(typeof(Math).GetMethod("Abs", new[] { typeof(long) }));
        var minLongMethod = context.Importer.Import(typeof(Math).GetMethod("Min", new[] { typeof(long), typeof(long) }));
        var maxLongMethod = context.Importer.Import(typeof(Math).GetMethod("Max", new[] { typeof(long), typeof(long) }));

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

                    if (instruction.IsLdcI4())
                    {
                        var operand = instruction.GetLdcI4Value();

                        if (operand >= 0)
                        {
                            method.Body.Instructions.Insert(i + 1, OpCodes.Call.ToInstruction(absMethod));
                        }

                        var neg = NumberUtils.Random.Next(2, 65);
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

                        var randomValue = NumberUtils.Random.Next();

                        for (;;)
                        {
                            try
                            {
                                _ = checked(operand + randomValue);
                                break;
                            }
                            catch (OverflowException)
                            {
                                randomValue = NumberUtils.Random.Next(randomValue);
                            }
                        }

                        var inst = OpCodes.Ldc_I4.ToInstruction(randomValue);
                        method.Body.Instructions.Insert(i + 1, inst);
                        method.Body.Instructions.Insert(i + 2, OpCodes.Add.ToInstruction());
                        method.Body.Instructions.Insert(i + 3, inst);
                        method.Body.Instructions.Insert(i + 4, OpCodes.Sub.ToInstruction());
                    }
                    else if (instruction.OpCode == OpCodes.Ldc_I8)
                    {
                        var operand = Convert.ToInt64(instruction.Operand);

                        if (operand >= 0)
                        {
                            method.Body.Instructions.Insert(i + 1, OpCodes.Call.ToInstruction(absLongMethod));
                        }

                        var neg = NumberUtils.Random.Next(2, 65);
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
                            method.Body.Instructions.Insert(i + 1, OpCodes.Ldc_I8.ToInstruction(1L));
                            method.Body.Instructions.Insert(i + 2, OpCodes.Call.ToInstruction(maxLongMethod));
                        }

                        if (operand < long.MaxValue)
                        {
                            method.Body.Instructions.Insert(i + 1, OpCodes.Ldc_I8.ToInstruction(long.MaxValue));
                            method.Body.Instructions.Insert(i + 2, OpCodes.Call.ToInstruction(minLongMethod));
                        }

                        var randomValue = NumberUtils.Random.NextInt64();

                        for (;;)
                        {
                            try
                            {
                               _ = checked(operand + randomValue);
                               break;
                            }
                            catch (OverflowException)
                            {
                                randomValue = NumberUtils.Random.NextInt64(randomValue);
                            }
                        }

                        var inst = OpCodes.Ldc_I8.ToInstruction(randomValue);
                        method.Body.Instructions.Insert(i + 1, inst);
                        method.Body.Instructions.Insert(i + 2, OpCodes.Add.ToInstruction());
                        method.Body.Instructions.Insert(i + 3, inst);
                        method.Body.Instructions.Insert(i + 4, OpCodes.Sub.ToInstruction());
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