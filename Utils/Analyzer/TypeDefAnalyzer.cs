using dnlib.DotNet;

namespace LoGiC.NET.Utils.Analyzer
{
    /// <summary>
    /// This class will analyze a type def.
    /// </summary>
	public class TypeDefAnalyzer : DefAnalyzer
	{
		public override bool Execute(object context)
		{
			TypeDef type = (TypeDef)context;
			if (type.IsRuntimeSpecialName)
				return false;
			if (type.IsGlobalModuleType)
				return false;
            if (type.IsWindowsRuntime)
                return false;
            return true;
		}
	}
}