using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using LoGiC.NET.Utils;

namespace LoGiC.NET.Protections
{
    public class StringEncryption : Randomizer
    {
        private static int Amount { get; set; }

        public static void Execute()
        {
            MethodDef strings = CreateReturnMethodDef();
            Program.Module.GlobalType.Methods.Add(strings);

            foreach (TypeDef type in Program.Module.Types)
            {
                if (type.IsGlobalModuleType) continue;
                foreach (MethodDef method in type.Methods)
                {
                    if (!method.HasBody) continue;
                    for (int i = 0; i < method.Body.Instructions.Count; i++)
                        if (method.Body.Instructions[i].OpCode == OpCodes.Ldstr)
                        {
                            string operand = method.Body.Instructions[i].Operand.ToString();
                            method.Body.Instructions[i].Operand = Convert.ToBase64String(Encoding.UTF8.GetBytes(operand));
                            method.Body.Instructions.Insert(i + 1, OpCodes.Call.ToInstruction(strings));
                            ++Amount;
                        }
                }
            }

            Console.WriteLine($"Encrypted {Amount} strings.");
        }

        private static MethodDef CreateReturnMethodDef()
        {
            MethodDef newMethod = new MethodDefUser(GenerateRandomString(Next(700, 500)),
                    MethodSig.CreateStatic(Program.Module.CorLibTypes.String, Program.Module.CorLibTypes.String),
                    MethodImplAttributes.IL | MethodImplAttributes.Managed,
                    MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig
                    | MethodAttributes.ReuseSlot)
            { Body = new CilBody() };

            newMethod.Body.Instructions.Add(OpCodes.Nop.ToInstruction());
            newMethod.Body.Instructions.Add(
                OpCodes.Call.ToInstruction(Program.Module.Import(typeof(Encoding).GetMethod("get_UTF8"))));
            newMethod.Body.Instructions.Add(OpCodes.Ldarg_0.ToInstruction());
            newMethod.Body.Instructions.Add(
                OpCodes.Call.ToInstruction(Program.Module.Import(typeof(Convert).GetMethod("FromBase64String",
                new Type[] { typeof(string) }))));
            newMethod.Body.Instructions.Add(
                OpCodes.Callvirt.ToInstruction(Program.Module.Import(typeof(Encoding).GetMethod("GetString",
                new Type[] { typeof(byte[]) }))));
            newMethod.Body.Instructions.Add(OpCodes.Ret.ToInstruction());
            return newMethod;
        }
    }
}
