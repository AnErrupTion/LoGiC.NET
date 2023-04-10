using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System.Collections.Generic;
using System.Linq;

namespace LoGiC.NET.Utils
{
    /// <summary>
    /// The class that contains extensions for MethodDefs. It's used for the 'ProxyAdder' method.
    /// </summary>
    public static class ProxyExtension
    {
        public static MethodDef CloneSignature(this MethodDef from, MethodDef to)
        {
            to.Attributes = from.Attributes;
            if (from.IsHideBySig) to.IsHideBySig = true;
            if (from.IsStatic) to.IsStatic = true;
            return to;
        }

        public static MethodDef CopyMethod(this MethodDef originMethod, ModuleDef mod)
        {
            InjectContext ctx = new InjectContext(mod, mod);

            MethodDefUser newMethodDef = new MethodDefUser
            {
                Signature = ctx.Importer.Import(originMethod.Signature),
                Name = Randomizer.String(MemberRenamer.StringLength())
            };
            newMethodDef.Parameters.UpdateParameterTypes();

            if (originMethod.ImplMap != null) newMethodDef.ImplMap = new ImplMapUser(new ModuleRefUser(ctx.TargetModule,
                    originMethod.ImplMap.Module.Name), originMethod.ImplMap.Name, originMethod.ImplMap.Attributes);

            foreach (CustomAttribute ca in originMethod.CustomAttributes)
                newMethodDef.CustomAttributes.Add(new CustomAttribute((ICustomAttributeType)
                    ctx.Importer.Import(ca.Constructor)));

            if (originMethod.HasBody)
            {
                newMethodDef.Body = new CilBody()
                {
                    InitLocals = originMethod.Body.InitLocals,
                    MaxStack = originMethod.Body.MaxStack
                };

                Dictionary<object, object> bodyMap = new Dictionary<object, object>();

                foreach (Local local in originMethod.Body.Variables)
                {
                    Local newLocal = new Local(ctx.Importer.Import(local.Type));
                    newMethodDef.Body.Variables.Add(newLocal);
                    newLocal.Name = local.Name;

                    bodyMap[local] = newLocal;
                }

                foreach (Instruction instr in originMethod.Body.Instructions)
                {
                    Instruction newInstr = new Instruction(instr.OpCode, instr.Operand)
                    { SequencePoint = instr.SequencePoint };

                    if (newInstr.Operand is IType) newInstr.Operand = ctx.Importer.Import((IType)newInstr.Operand);
                    else if (newInstr.Operand is IMethod) newInstr.Operand = ctx.Importer.Import((IMethod)newInstr.Operand);
                    else if (newInstr.Operand is IField) newInstr.Operand = ctx.Importer.Import((IField)newInstr.Operand);

                    newMethodDef.Body.Instructions.Add(newInstr);
                    bodyMap[instr] = newInstr;
                }

                foreach (Instruction instr in newMethodDef.Body.Instructions)
                    if (instr.Operand != null && bodyMap.ContainsKey(instr.Operand)) instr.Operand = bodyMap[instr.Operand];
                    else if (instr.Operand is Instruction[]) instr.Operand = ((Instruction[])instr.Operand).Select(
                        target => (Instruction)bodyMap[target]).ToArray();

                foreach (ExceptionHandler eh in originMethod.Body.ExceptionHandlers)
                    newMethodDef.Body.ExceptionHandlers.Add(new ExceptionHandler(eh.HandlerType)
                    {
                        CatchType = eh.CatchType == null ? null : ctx.Importer.Import(eh.CatchType),
                        TryStart = (Instruction)bodyMap[eh.TryStart],
                        TryEnd = (Instruction)bodyMap[eh.TryEnd],
                        HandlerStart = (Instruction)bodyMap[eh.HandlerStart],
                        HandlerEnd = (Instruction)bodyMap[eh.HandlerEnd],
                        FilterStart = eh.FilterStart == null ? null : (Instruction)bodyMap[eh.FilterStart]
                    });

                newMethodDef.Body.SimplifyMacros(newMethodDef.Parameters);
            }

            return newMethodDef;
        }
    }
}
