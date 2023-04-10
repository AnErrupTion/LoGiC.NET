using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace LoGiC.NET.Utils
{
    public class Watermark
    {
        public static void AddAttribute()
        {
            TypeRef attrRef = Program.Module.CorLibTypes.GetTypeRef("System", "Attribute");
            TypeDefUser attrType = new TypeDefUser(string.Empty, "LoGiCdotNetAttribute", attrRef);
            Program.Module.Types.Add(attrType);
            MethodDefUser ctor = new MethodDefUser(".ctor", MethodSig.CreateInstance(Program.Module
                .CorLibTypes.Void, Program.Module.CorLibTypes.String), MethodImplAttributes.Managed,
                MethodAttributes.HideBySig | MethodAttributes.Public | MethodAttributes.SpecialName |
                MethodAttributes.RTSpecialName)
            {
                Body = new CilBody { MaxStack = 1 }
            };

            ctor.Body.Instructions.Add(OpCodes.Ldarg_0.ToInstruction());
            ctor.Body.Instructions.Add(OpCodes.Call.ToInstruction(new MemberRefUser(Program.Module, ".ctor",
                MethodSig.CreateInstance(Program.Module.CorLibTypes.Void), attrRef)));
            ctor.Body.Instructions.Add(OpCodes.Ret.ToInstruction());
            attrType.Methods.Add(ctor);

            CustomAttribute attr = new CustomAttribute(ctor);
            attr.ConstructorArguments.Add(new CAArgument(Program.Module.CorLibTypes.String, $"Obfuscated" +
                $" with {Reference.Name} version {Reference.Version}."));
            Program.Module.CustomAttributes.Add(attr);
        }
    }
}
