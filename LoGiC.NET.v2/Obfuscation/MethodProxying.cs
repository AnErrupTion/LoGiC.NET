using dnlib.DotNet;
using dnlib.DotNet.Emit;
using LoGiC.NET.v2.Utils;

namespace LoGiC.NET.v2.Obfuscation;

// TODO: Fix
public sealed class MethodProxying : BaseObfuscation
{
    public override string Name => "Method proxying";

    private uint _proxiedMethods;

    public override void Run(ObfuscationContext context)
    {
        foreach (var type in context.Module.Types)
        {
            var methods = new List<MethodDef>(type.Methods);

            foreach (var method in methods)
            {
                if (!method.HasBody)
                {
                    Terminal.Warn($"Method has no body: {method.FullName}");
                    continue;
                }

                foreach (var instruction in method.Body.Instructions)
                {
                    if (instruction.OpCode != OpCodes.Call || instruction.Operand is not MethodDef methodToCall || !methodToCall.FullName.Contains(context.Module.Assembly.Name))
                    {
                        continue;
                    }

                    var newMethod = new MethodDefUser(NumberUtils.Random.NextInt64().ToString(), methodToCall.MethodSig, methodToCall.Attributes);
                    var newMethodBody = new CilBody();

                    for (ushort j = 0; j < newMethod.Parameters.Count; j++)
                    {
                        newMethodBody.Instructions.Add(new Instruction(OpCodes.Ldarg, j));
                    }

                    newMethodBody.Instructions.Add(OpCodes.Call.ToInstruction(newMethod));
                    newMethodBody.Instructions.Add(OpCodes.Ret.ToInstruction());
                    newMethodBody.OptimizeMacros();

                    type.Methods.Add(newMethod);
                }

                _proxiedMethods++;
            }
        }

        Terminal.Info($"Proxied {_proxiedMethods} methods");
    }
}