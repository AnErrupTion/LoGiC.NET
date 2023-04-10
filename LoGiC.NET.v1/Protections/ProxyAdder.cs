using System;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using LoGiC.NET.Utils;

namespace LoGiC.NET.Protections
{
    public class ProxyAdder : Protection
    {
        public ProxyAdder()
        {
            Name = "Proxy Adder";
        }

        /// <summary>
        /// The intensity of the proxy calls. The more the intensity is, the more proxy calls will be added!
        /// </summary>
        public static int Intensity { get; set; } = 2;

        /// <summary>
        /// The amount of added proxy calls.
        /// </summary>
        private int Amount { get; set; }

        // Thanks to the BasicProxyObfuscator project by XenocodeRCE on GitHub!

        /// <summary>
        /// Execution of the 'ProxyAdder' method. It'll add proxy calls, basically each proxy call will call another method that will call another method, etc. until it calls a real method (example : InitializeComponent).
        /// </summary>
        public override void Execute()
        {
            for (int o = 0; o < Intensity; o++)
                foreach (TypeDef t in Program.Module.Types)
                    for (int i = 0; i < t.Methods.Count; i++)
                    {
                        MethodDef m = t.Methods[i];

                        if (!m.HasBody)
                            continue;

                        for (int z = 0; z < m.Body.Instructions.Count; z++)
                            if (m.Body.Instructions[z].OpCode == OpCodes.Call)
                            {
                                if (!(m.Body.Instructions[z].Operand is MethodDef targetMethod) || !targetMethod.FullName.Contains(Program.Module.Assembly.Name) || targetMethod.Parameters.Count > 4)
                                    continue;

                                MethodDef newMeth = targetMethod.CopyMethod(Program.Module);
                                targetMethod.DeclaringType.Methods.Add(newMeth);
                                targetMethod.CloneSignature(newMeth);

                                CilBody body = new CilBody();
                                body.Instructions.Add(OpCodes.Nop.ToInstruction());

                                if (targetMethod.Parameters.Count > 0)
                                    for (int x = 0; x < targetMethod.Parameters.Count; x++)
                                        switch (x)
                                        {
                                            case 0:
                                                body.Instructions.Add(OpCodes.Ldarg_0.ToInstruction());
                                                break;
                                            case 1:
                                                body.Instructions.Add(OpCodes.Ldarg_1.ToInstruction());
                                                break;
                                            case 2:
                                                body.Instructions.Add(OpCodes.Ldarg_2.ToInstruction());
                                                break;
                                            case 3:
                                                body.Instructions.Add(OpCodes.Ldarg_3.ToInstruction());
                                                break;
                                        }

                                body.Instructions.Add(OpCodes.Call.ToInstruction(newMeth));
                                body.Instructions.Add(OpCodes.Ret.ToInstruction());

                                targetMethod.Body = body;
                                ++Amount;
                            }
                    }

            Console.WriteLine($"  Added {Amount} proxy calls.");
        }
    }
}
