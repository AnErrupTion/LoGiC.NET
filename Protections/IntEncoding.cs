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
    public class IntEncoding : Randomizer
    {
        /// <summary>
        /// The amount of encoded ints.
        /// </summary>
        private static int Amount { get; set; }

        /// <summary>
        /// Execution of the 'IntEncoding' method. It'll encodes the integers within different methods.
        /// Absolute : This method will add Math.Abs(int) before each integer.
        /// StringLen : This method will replace each integer by their string equivalent.
        /// </summary>
        public static void Execute()
        {
            foreach (TypeDef type in Program.Module.Types)
                foreach (MethodDef method in type.Methods)
                {
                    if (!method.HasBody) continue;
                    for (int i = 0; i < method.Body.Instructions.Count; i++)
                        if (method.Body.Instructions[i].IsLdcI4())
                        {
                            // The Absolute method.
                            int operand = method.Body.Instructions[i].GetLdcI4Value();
                            if (operand <= 0) continue; // Prevents errors.
                            method.Body.Instructions.Insert(i + 1, OpCodes.Call.ToInstruction(
                                Program.Module.Import(typeof(Math).GetMethod("Abs", new Type[] { typeof(int) }))));

                            // The String Length method.
                            // To fix
                            /*method.Body.Instructions[i].OpCode = OpCodes.Ldstr;
                            method.Body.Instructions[i].Operand = GenerateRandomString(operand);
                            method.Body.Instructions.Insert(i + 1, OpCodes.Call.ToInstruction(
                                Program.Module.Import(typeof(string).GetMethod("get_Length"))));*/

                            ++Amount;
                        }
                }

            Console.WriteLine($"  Encoded {Amount} ints.");
        }
    }
}
