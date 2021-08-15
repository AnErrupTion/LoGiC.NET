using System;
using LoGiC.NET.Utils;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace LoGiC.NET.Protections
{
    public class JunkMethods : Randomizer
    {
        /// <summary>
        /// The amount of added junk methods.
        /// </summary>
        private static int Amount;
        
        /// <summary>
        /// This obfuscation will add random junk methods to make the code harder to decrypt to people if they think the junk methods are actually used.
        /// </summary>
        public static void Execute()
        {
            foreach (TypeDef type in Program.Module.Types)
                for (int i = 0; i < MemberRenamer.StringLength(); i++)
                {
                    MethodDef strings = CreateReturnMethodDef(String(MemberRenamer.StringLength()));
                    MethodDef ints = CreateReturnMethodDef(MemberRenamer.StringLength());

                    type.Methods.Add(strings);
                    type.Methods.Add(ints);

                    Amount += 2;
                }

            Console.WriteLine($"  Added {Amount} junk methods.");
        }

        /// <summary>
		/// The return value for the randomly generated method. It can be an integer or a string.
		/// </summary>
		private static MethodDef CreateReturnMethodDef(object value)
        {
            CorLibTypeSig corlib = null;
            if (value is int)
                corlib = Program.Module.CorLibTypes.Int32;
            else if (value is string)
                corlib = Program.Module.CorLibTypes.String;

            MethodDef newMethod = new MethodDefUser(String(MemberRenamer.StringLength()), MethodSig.CreateStatic(corlib),
                    MethodImplAttributes.IL | MethodImplAttributes.Managed,
                    MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig)
            {
                Body = new CilBody()
            };

            if (value is int)
                newMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, Convert.ToInt32(value)));
            else if (value is string)
                newMethod.Body.Instructions.Add(Instruction.Create(OpCodes.Ldstr, value.ToString()));

            newMethod.Body.Instructions.Add(OpCodes.Ret.ToInstruction());

            return newMethod;
        }
    }
}
