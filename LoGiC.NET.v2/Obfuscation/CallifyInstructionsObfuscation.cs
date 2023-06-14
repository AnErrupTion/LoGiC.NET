using dnlib.DotNet;
using dnlib.DotNet.Emit;
using LoGiC.NET.v2.Utils;

namespace LoGiC.NET.v2.Obfuscation;

// TODO: Fix
public sealed class CallifyInstructionsObfuscation : BaseObfuscation
{
    public override string Name => "Callify instructions";

    public override void Run(ObfuscationContext context)
    {
        foreach (var type in context.Module.Types)
        {
            foreach (var method in type.Methods)
            {
                if (!method.HasBody || method.Body.HasExceptionHandlers)
                {
                    Terminal.Warn($"Method has no body: {method.FullName}");
                    continue;
                }

                var instructions = new Instruction[method.Body.Instructions.Count];

                for (var i = 0; i < method.Body.Instructions.Count; i++)
                {
                    var instruction = method.Body.Instructions[i];

                    TypeSig returnType;

                    switch (instruction.OpCode.Code)
                    {
                        case Code.Ldc_I4:
                        case Code.Ldc_I4_S:
                        case Code.Ldc_I4_M1:
                        case Code.Ldc_I4_0:
                        case Code.Ldc_I4_1:
                        case Code.Ldc_I4_2:
                        case Code.Ldc_I4_3:
                        case Code.Ldc_I4_4:
                        case Code.Ldc_I4_5:
                        case Code.Ldc_I4_6:
                        case Code.Ldc_I4_7:
                        case Code.Ldc_I4_8:
                        {
                            returnType = context.Module.CorLibTypes.Int32;
                            break;
                        }
                        default:
                        {
                            Terminal.Warn($"Unimplemented OpCode: {instruction.OpCode.Code}");
                            instructions[i] = instruction;
                            continue;
                        }
                    }

                    var body = new CilBody();
                    body.Instructions.Add(instruction);
                    body.Instructions.Add(Instruction.Create(OpCodes.Ret));

                    var newMethod = new MethodDefUser(NumberUtils.Random.NextInt64().ToString().ToString(),
                        MethodSig.CreateStatic(returnType))
                    {
                        Body = body
                    };

                    context.Module.Types[0].Methods.Add(newMethod);

                    instructions[i] = Instruction.Create(OpCodes.Call, newMethod);
                }

                // Correct branch instructions
                for (var i = 0; i < instructions.Length; i++)
                {
                    var instruction = instructions[i];

                    if (instruction.OpCode.OperandType is not OperandType.InlineBrTarget and not OperandType.ShortInlineBrTarget)
                    {
                        continue;
                    }

                    var targetInstruction = instruction.Operand as Instruction;
                    var targetIndex = -1;

                    for (var j = 0; j < method.Body.Instructions.Count; j++)
                    {
                        var inst = method.Body.Instructions[j];

                        if (inst.Offset == targetInstruction?.Offset)
                        {
                            targetIndex = j;
                            break;
                        }
                    }

                    if (targetIndex is -1)
                        throw new InvalidOperationException("Catastrophic failure");

                    instructions[i] = instruction.OpCode.Code switch
                    {
                        Code.Br or Code.Br_S => Instruction.Create(OpCodes.Br, instructions[targetIndex]),
                        Code.Brtrue or Code.Brtrue_S => Instruction.Create(OpCodes.Brtrue, instructions[targetIndex]),
                        Code.Brfalse or Code.Brfalse_S => Instruction.Create(OpCodes.Brfalse, instructions[targetIndex]),
                        Code.Beq or Code.Beq_S => Instruction.Create(OpCodes.Beq, instructions[targetIndex]),
                        Code.Bne_Un or Code.Bne_Un_S => Instruction.Create(OpCodes.Bne_Un, instructions[targetIndex]),
                        Code.Blt or Code.Blt_S => Instruction.Create(OpCodes.Blt, instructions[targetIndex]),
                        Code.Blt_Un or Code.Blt_Un_S => Instruction.Create(OpCodes.Blt_Un, instructions[targetIndex]),
                        Code.Ble or Code.Ble_S => Instruction.Create(OpCodes.Ble, instructions[targetIndex]),
                        Code.Ble_Un or Code.Ble_Un_S => Instruction.Create(OpCodes.Ble_Un, instructions[targetIndex]),
                        Code.Bgt or Code.Bgt_S => Instruction.Create(OpCodes.Bgt, instructions[targetIndex]),
                        Code.Bgt_Un or Code.Bgt_Un_S => Instruction.Create(OpCodes.Bgt_Un, instructions[targetIndex]),
                        Code.Bge or Code.Bge_S => Instruction.Create(OpCodes.Bge, instructions[targetIndex]),
                        Code.Bge_Un or Code.Bge_Un_S => Instruction.Create(OpCodes.Bge_Un, instructions[targetIndex]),
                        _ => throw new NotImplementedException(instruction.OpCode.Code.ToString())
                    };
                }

                method.Body.Instructions.Clear();
                foreach (var instruction in instructions)
                {
                    method.Body.Instructions.Add(instruction);
                }
            }
        }
    }
}