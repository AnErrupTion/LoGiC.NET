using System;
using LoGiC.NET.Utils;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace LoGiC.NET.Protections
{
    public class JunkDefs : Protection
    {
        public JunkDefs()
        {
            Name = "Junk Defs";
        }

        /// <summary>
        /// The amount of added junk defs.
        /// </summary>
        private int Amount;
        
        /// <summary>
        /// This obfuscation will add random junk defs to make the code harder to decrypt to people if they think the junk methods are actually used.
        /// </summary>
        public override void Execute()
        {
            // Add junk types
            for (int i = 0; i < MemberRenamer.StringLength(); i++)
            {
                TypeDef type = new TypeDefUser(Randomizer.String(MemberRenamer.StringLength())) { Namespace = string.Empty };
                Program.Module.Types.Add(type);

                Amount++;
            }

            // Add junk methods
            foreach (TypeDef type in Program.Module.Types)
                for (int i = 0; i < MemberRenamer.StringLength(); i++)
                {
                    MethodDef strings = CreateNewJunkMethod(Randomizer.String(MemberRenamer.StringLength()));
                    MethodDef ints = CreateNewJunkMethod(MemberRenamer.StringLength());

                    type.Methods.Add(strings);
                    type.Methods.Add(ints);

                    Amount += 2;
                }

            Console.WriteLine($"  Added {Amount} junk defs.");
        }

        /// <summary>
		/// Creates a new method with a return value.
		/// </summary>
		private MethodDef CreateNewJunkMethod(object value)
        {
            CorLibTypeSig corlib = null;
            if (value is int)
                corlib = Program.Module.CorLibTypes.Int32;
            else if (value is string)
                corlib = Program.Module.CorLibTypes.String;

            MethodDef newMethod = new MethodDefUser(Randomizer.String(MemberRenamer.StringLength()), MethodSig.CreateStatic(corlib),
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
