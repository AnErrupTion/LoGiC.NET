using dnlib.DotNet;

namespace LoGiC.NET.Utils.Analyzer
{
    /// <summary>
    /// This class will analyze a method def.
    /// </summary>
	public class MethodDefAnalyzer : DefAnalyzer
	{
		public override bool Execute(object context)
		{
			MethodDef method = (MethodDef)context;
			return !method.IsRuntimeSpecialName && !method.DeclaringType.IsForwarder && !method.IsConstructor && !method.IsVirtual;
		}
	}
}