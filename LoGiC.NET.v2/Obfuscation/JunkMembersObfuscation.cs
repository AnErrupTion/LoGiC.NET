using System.Diagnostics;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using LoGiC.NET.v2.Utils;

namespace LoGiC.NET.v2.Obfuscation;

public sealed class JunkMembersObfuscation : BaseObfuscation
{
    public override string Name => "Junk definitions";

    private uint _junkMembersAdded;

    public override void Run(ObfuscationContext context)
    {
        // Add junk types
        for (var i = 0; i < NumberUtils.Random.Next(10, 33); i++)
        {
            var newType = new TypeDefUser(NumberUtils.Random.NextInt64().ToString(), NumberUtils.Random.NextInt64().ToString());

            context.Module.Types.Add(newType);

            _junkMembersAdded++;
        }

        // Add junk methods, and call them
        foreach (var type in context.Module.Types)
        {
            for (var i = 0; i < NumberUtils.Random.Next(10, 33); i++)
            {
                MethodDefUser newMethod;

                switch (NumberUtils.Random.Next(0, 3))
                {
                    case 0: // Int32
                    {
                        var body = new CilBody();
                        body.Instructions.Add(OpCodes.Ldc_I4.ToInstruction(NumberUtils.Random.Next()));
                        body.Instructions.Add(OpCodes.Ret.ToInstruction());
                        body.OptimizeMacros();

                        newMethod = new MethodDefUser(
                            NumberUtils.Random.NextInt64().ToString(),
                            MethodSig.CreateStatic(context.Module.CorLibTypes.Int32),
                            MethodImplAttributes.IL | MethodImplAttributes.Managed,
                            MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig)
                        {
                            Body = body
                        };
                        break;
                    }
                    case 1: // Int64
                    {
                        var body = new CilBody();
                        body.Instructions.Add(OpCodes.Ldc_I8.ToInstruction(NumberUtils.Random.NextInt64()));
                        body.Instructions.Add(OpCodes.Ret.ToInstruction());
                        body.OptimizeMacros();

                        newMethod = new MethodDefUser(
                            NumberUtils.Random.NextInt64().ToString(),
                            MethodSig.CreateStatic(context.Module.CorLibTypes.Int64),
                            MethodImplAttributes.IL | MethodImplAttributes.Managed,
                            MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig)
                        {
                            Body = body
                        };
                        break;
                    }
                    case 2: // String
                    {
                        var body = new CilBody();
                        body.Instructions.Add(OpCodes.Ldstr.ToInstruction(NumberUtils.Random.NextInt64().ToString()));
                        body.Instructions.Add(OpCodes.Ret.ToInstruction());
                        body.OptimizeMacros();

                        newMethod = new MethodDefUser(
                            NumberUtils.Random.NextInt64().ToString(),
                            MethodSig.CreateStatic(context.Module.CorLibTypes.String),
                            MethodImplAttributes.IL | MethodImplAttributes.Managed,
                            MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig)
                        {
                            Body = body
                        };
                        break;
                    }
                    default: throw new UnreachableException();
                }

                type.Methods.Add(newMethod);

                /*var filteredMethods = (from aType in context.Module.Types from aMethod in aType.Methods where aMethod.HasBody && aMethod.FullName != newMethod.FullName select aMethod).ToArray();
                var randomMethod = filteredMethods[NumberUtils.Random.Next(0, filteredMethods.Length)];

                // TODO: Why does it say it couldn't load the type because "the parent does not exist"?
                randomMethod.Body.Instructions.Insert(0, OpCodes.Call.ToInstruction(newMethod));
                randomMethod.Body.Instructions.Insert(1, OpCodes.Pop.ToInstruction());*/

                _junkMembersAdded++;
            }
        }

        Terminal.Info($"Added {_junkMembersAdded} junk members.");
    }
}