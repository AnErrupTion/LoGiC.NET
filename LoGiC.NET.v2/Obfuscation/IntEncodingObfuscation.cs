using dnlib.DotNet;
using dnlib.DotNet.Emit;
using LoGiC.NET.v2.Utils;

namespace LoGiC.NET.v2.Obfuscation;

public sealed class IntEncodingObfuscation : BaseObfuscation
{
    public override string Name => "Int encoding";

    private uint _encodedInts;

    private IMethod
        _absIntMethod, _minIntMethod, _maxIntMethod,
        _absLongMethod, _minLongMethod, _maxLongMethod,
        _absFloatMethod, _minFloatMethod, _maxFloatMethod,
        _absDoubleMethod, _minDoubleMethod, _maxDoubleMethod;

    public override void Run(ObfuscationContext context)
    {
        _absIntMethod = context.Importer.Import(typeof(Math).GetMethod("Abs", new[] { typeof(int) }));
        _minIntMethod = context.Importer.Import(typeof(Math).GetMethod("Min", new[] { typeof(int), typeof(int) }));
        _maxIntMethod = context.Importer.Import(typeof(Math).GetMethod("Max", new[] { typeof(int), typeof(int) }));

        _absLongMethod = context.Importer.Import(typeof(Math).GetMethod("Abs", new[] { typeof(long) }));
        _minLongMethod = context.Importer.Import(typeof(Math).GetMethod("Min", new[] { typeof(long), typeof(long) }));
        _maxLongMethod = context.Importer.Import(typeof(Math).GetMethod("Max", new[] { typeof(long), typeof(long) }));

        _absFloatMethod = context.Importer.Import(typeof(Math).GetMethod("Abs", new[] { typeof(float) }));
        _minFloatMethod = context.Importer.Import(typeof(Math).GetMethod("Min", new[] { typeof(float), typeof(float) }));
        _maxFloatMethod = context.Importer.Import(typeof(Math).GetMethod("Max", new[] { typeof(float), typeof(float) }));

        _absDoubleMethod = context.Importer.Import(typeof(Math).GetMethod("Abs", new[] { typeof(double) }));
        _minDoubleMethod = context.Importer.Import(typeof(Math).GetMethod("Min", new[] { typeof(double), typeof(double) }));
        _maxDoubleMethod = context.Importer.Import(typeof(Math).GetMethod("Max", new[] { typeof(double), typeof(double) }));

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
                        EncodeInt(method, instruction.GetLdcI4Value(), i);
                    }
                    else if (instruction.OpCode == OpCodes.Ldc_I8)
                    {
                        EncodeLong(method, Convert.ToInt64(instruction.Operand), i);
                    }
                    else if (instruction.OpCode == OpCodes.Ldc_R4)
                    {
                        EncodeFloat(method, Convert.ToSingle(instruction.Operand), i);
                    }
                    else if (instruction.OpCode == OpCodes.Ldc_R8)
                    {
                        EncodeDouble(method, Convert.ToSingle(instruction.Operand), i);
                    }
                }

                method.Body.SimplifyBranches();
                method.Body.OptimizeBranches();

                _encodedInts++;
            }
        }

        Terminal.Info($"Encoded {_encodedInts} ints");
    }

    private void EncodeInt(MethodDef method, int operand, int i)
    {
        if (operand >= 0)
        {
            method.Body.Instructions.Insert(i + 1, OpCodes.Call.ToInstruction(_absIntMethod));
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
            method.Body.Instructions.Insert(i + 2, OpCodes.Call.ToInstruction(_maxIntMethod));
        }

        if (operand < int.MaxValue)
        {
            method.Body.Instructions.Insert(i + 1, OpCodes.Ldc_I4.ToInstruction(int.MaxValue));
            method.Body.Instructions.Insert(i + 2, OpCodes.Call.ToInstruction(_minIntMethod));
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

    private void EncodeLong(MethodDef method, long operand, int i)
    {
        if (operand >= 0)
        {
            method.Body.Instructions.Insert(i + 1, OpCodes.Call.ToInstruction(_absLongMethod));
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
            method.Body.Instructions.Insert(i + 2, OpCodes.Call.ToInstruction(_maxLongMethod));
        }

        if (operand < long.MaxValue)
        {
            method.Body.Instructions.Insert(i + 1, OpCodes.Ldc_I8.ToInstruction(long.MaxValue));
            method.Body.Instructions.Insert(i + 2, OpCodes.Call.ToInstruction(_minLongMethod));
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
    
    private void EncodeFloat(MethodDef method, float operand, int i)
    {
        if (operand >= 0)
        {
            method.Body.Instructions.Insert(i + 1, OpCodes.Call.ToInstruction(_absFloatMethod));
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
            method.Body.Instructions.Insert(i + 1, OpCodes.Ldc_R4.ToInstruction(1F));
            method.Body.Instructions.Insert(i + 2, OpCodes.Call.ToInstruction(_maxFloatMethod));
        }

        if (operand < float.MaxValue)
        {
            method.Body.Instructions.Insert(i + 1, OpCodes.Ldc_R4.ToInstruction(float.MaxValue));
            method.Body.Instructions.Insert(i + 2, OpCodes.Call.ToInstruction(_minFloatMethod));
        }

        var randomValue = NumberUtils.Random.NextSingle();
        var inst = OpCodes.Ldc_R4.ToInstruction(randomValue);

        method.Body.Instructions.Insert(i + 1, inst);
        method.Body.Instructions.Insert(i + 2, OpCodes.Add.ToInstruction());
        method.Body.Instructions.Insert(i + 3, inst);
        method.Body.Instructions.Insert(i + 4, OpCodes.Sub.ToInstruction());
    }
    
    private void EncodeDouble(MethodDef method, float operand, int i)
    {
        if (operand >= 0)
        {
            method.Body.Instructions.Insert(i + 1, OpCodes.Call.ToInstruction(_absDoubleMethod));
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
            method.Body.Instructions.Insert(i + 1, OpCodes.Ldc_R8.ToInstruction(1D));
            method.Body.Instructions.Insert(i + 2, OpCodes.Call.ToInstruction(_maxDoubleMethod));
        }

        if (operand < double.MaxValue)
        {
            method.Body.Instructions.Insert(i + 1, OpCodes.Ldc_R8.ToInstruction(double.MaxValue));
            method.Body.Instructions.Insert(i + 2, OpCodes.Call.ToInstruction(_minDoubleMethod));
        }

        var randomValue = NumberUtils.Random.NextDouble();
        var inst = OpCodes.Ldc_R8.ToInstruction(randomValue);

        method.Body.Instructions.Insert(i + 1, inst);
        method.Body.Instructions.Insert(i + 2, OpCodes.Add.ToInstruction());
        method.Body.Instructions.Insert(i + 3, inst);
        method.Body.Instructions.Insert(i + 4, OpCodes.Sub.ToInstruction());
    }
}