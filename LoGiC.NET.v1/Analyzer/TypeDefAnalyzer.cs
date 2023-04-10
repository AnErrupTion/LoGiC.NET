using dnlib.DotNet;

namespace LoGiC.NET.Utils.Analyzer
{
    public class TypeDefAnalyzer : DefAnalyzer
    {
        public override bool Execute(object context)
        {
            TypeDef type = (TypeDef)context;
            return !type.IsSpecialName && !type.IsWindowsRuntime && !type.IsForwarder && !type.IsRuntimeSpecialName;
        }
    }
}
