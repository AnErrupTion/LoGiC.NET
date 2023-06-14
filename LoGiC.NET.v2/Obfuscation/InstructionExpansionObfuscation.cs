using dnlib.DotNet;
using dnlib.DotNet.Emit;
using LoGiC.NET.v2.Utils;

namespace LoGiC.NET.v2.Obfuscation;

public sealed class InstructionExpansionObfuscation : BaseObfuscation
{
    public override string Name => "Instruction expansion";

    private MethodDef _method;
    private int _index;
    private uint _expandedInstructions;

    public override void Run(ObfuscationContext context)
    {
        foreach (var type in context.Module.Types)
        {
            foreach (var method in type.Methods)
            {
                if (!method.HasBody)
                {
                    Terminal.Warn($"Method has no body: {method.FullName}");
                    continue;
                }

                _method = method;
                _index = 0;

                while (_index < method.Body.Instructions.Count)
                {
                    var instruction = method.Body.Instructions[_index];
                    var skipInstruction = false;

                    foreach (var handler in method.Body.ExceptionHandlers)
                    {
                        if (handler.HandlerStart == instruction || handler.HandlerEnd == instruction)
                        {
                            Terminal.Warn($"Instruction is the first or last instruction of an exception handler: {instruction}");
                            skipInstruction = true;
                            break;
                        }
                    }

                    if (skipInstruction)
                    {
                        _index++;
                        continue;
                    }

                    switch (instruction.OpCode.Code)
                    {
                        case Code.Ldc_I4_0: ExpandLdcI4(0); break;
                        case Code.Ldc_I4_1: ExpandLdcI4(1); break;
                        case Code.Ldc_I4_2: ExpandLdcI4(2); break;
                        case Code.Ldc_I4_3: ExpandLdcI4(3); break;
                        case Code.Ldc_I4_4: ExpandLdcI4(4); break;
                        case Code.Ldc_I4_5: ExpandLdcI4(5); break;
                        case Code.Ldc_I4_6: ExpandLdcI4(6); break;
                        case Code.Ldc_I4_7: ExpandLdcI4(7); break;
                        case Code.Ldc_I4_8: ExpandLdcI4(8); break;
                        case Code.Ldc_I4_S:
                        case Code.Ldc_I4: ExpandLdcI4(Convert.ToInt32(instruction.Operand)); break;
                        default: _index++; break;
                    }
                }
            }
        }

        Terminal.Info($"Expanded {_expandedInstructions} instructions.");
    }

    private void ExpandLdcI4(int value)
    {
        if (value < 0)
        {
            _index++;
            return;
        }

        var upperBound = int.MinValue;
        while (upperBound * 2 < value)
            upperBound++;

        var numbers = NumberUtils.GetAddOperationFor(value, 2, int.MinValue, upperBound);

        var instruction = _method.Body.Instructions[_index];
        instruction.OpCode = OpCodes.Ldc_I4;
        instruction.Operand = numbers[0];
        _method.Body.Instructions[_index++] = instruction;

        _method.Body.Instructions.Insert(_index++, Instruction.CreateLdcI4(numbers[1]));
        _method.Body.Instructions.Insert(_index++, Instruction.Create(OpCodes.Add));

        _expandedInstructions++;
    }
}