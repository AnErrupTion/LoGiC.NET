using dnlib.DotNet;

namespace LoGiC.NET.Utils.Analyzer
{
    /// <summary>
    /// This class will analyze an event def.
    /// </summary>
	public class EventDefAnalyzer : DefAnalyzer
    {
		public override bool Execute(object context)
		{
			EventDef ev = (EventDef)context;
			if (ev.IsRuntimeSpecialName)
				return false;
			return true;
		}
	}
}