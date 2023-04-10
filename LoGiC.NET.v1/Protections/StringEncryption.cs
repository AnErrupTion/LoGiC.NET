using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using LoGiC.NET.Utils;

namespace LoGiC.NET.Protections
{
    public class StringEncryption : Protection
    {
        public StringEncryption()
        {
            Name = "String Encryption";
        }

        private int Amount { get; set; }

        public override void Execute()
        {
            ModuleDefMD typeModule = ModuleDefMD.Load(typeof(StringDecoder).Module);
            TypeDef typeDef = typeModule.ResolveTypeDef(MDToken.ToRID(typeof(StringDecoder).MetadataToken));
            IEnumerable<IDnlibDef> members = InjectHelper.Inject(typeDef, Program.Module.GlobalType, Program.Module);
            MethodDef init = (MethodDef)members.Single(method => method.Name == "Decrypt");
            init.GetRenamed();

            foreach (MethodDef method in Program.Module.GlobalType.Methods)
                if (method.Name.Equals(".ctor"))
                {
                    Program.Module.GlobalType.Remove(method);
                    break;
                }

            foreach (TypeDef type in Program.Module.Types)
                foreach (MethodDef method in type.Methods)
                {
                    if (!method.HasBody)
                        continue;

                    method.Body.SimplifyBranches();

                    for (int i = 0; i < method.Body.Instructions.Count; i++)
                        if (method.Body.Instructions[i] != null && method.Body.Instructions[i].OpCode == OpCodes.Ldstr)
                        {
                            int key = Randomizer.Next();
                            object op = method.Body.Instructions[i].Operand;

                            if (op == null)
                                continue;

                            method.Body.Instructions[i].Operand = Encrypt(op.ToString(), key);
                            method.Body.Instructions.Insert(i + 1, OpCodes.Ldc_I4.ToInstruction(Randomizer.Next()));
                            method.Body.Instructions.Insert(i + 2, OpCodes.Ldc_I4.ToInstruction(key));
                            method.Body.Instructions.Insert(i + 3, OpCodes.Ldc_I4.ToInstruction(Randomizer.Next()));
                            method.Body.Instructions.Insert(i + 4, OpCodes.Ldc_I4.ToInstruction(Randomizer.Next()));
                            method.Body.Instructions.Insert(i + 5, OpCodes.Ldc_I4.ToInstruction(Randomizer.Next()));
                            method.Body.Instructions.Insert(i + 6, OpCodes.Call.ToInstruction(init));
                            
                            ++Amount;
                        }

                    method.Body.OptimizeBranches();
                }

            Console.WriteLine($"  Encrypted {Amount} strings.");
        }

        private string Encrypt(string str, int key)
        {
            //str = Convert.ToBase64String(Encoding.UTF32.GetBytes(str));
            /*I'm using UTF32, but you can
            also use UTF8 or Unicode for example for shorter encryption.*/
            //char[] chars = "*$,;:!ù^*&é\"'(-è_çà)".ToCharArray();

            //for (int i = 0; i < 5; i++)
            /*<-- this is how many times you will add every character from the
                array at a random position. 5 is just enough for what we want to do.*/
            //foreach (char c in chars) str = str.Insert(Next(str.Length), c.ToString());
            //return str;

            StringBuilder builder = new StringBuilder();
            foreach (char c in str.ToCharArray())
                builder.Append((char)(c + key));

            return builder.ToString();
        }
    }
}
