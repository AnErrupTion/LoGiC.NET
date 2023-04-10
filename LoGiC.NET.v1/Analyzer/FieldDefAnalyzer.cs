using dnlib.DotNet;

namespace LoGiC.NET.Utils.Analyzer
{
    /// <summary>
    /// This class will analyze a field def.
    /// </summary>
	public class FieldDefAnalyzer : DefAnalyzer
	{
		public override bool Execute(object context)
		{
			FieldDef field = (FieldDef)context;
			return !field.IsRuntimeSpecialName && !(field.IsLiteral && field.DeclaringType.IsEnum);
		}
	}
}