using dnlib.DotNet;

namespace LoGiC.NET.Utils.Analyzer
{
	public class PropertyDefAnalyzer : DefAnalyzer
	{
		public override bool Execute(object context)
		{
			PropertyDef propertyDef = (PropertyDef)context;
			return !propertyDef.IsRuntimeSpecialName && !propertyDef.IsEmpty && propertyDef.IsSpecialName;
		}
	}
}
