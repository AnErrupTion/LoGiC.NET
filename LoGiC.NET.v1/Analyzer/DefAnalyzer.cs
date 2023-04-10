namespace LoGiC.NET.Utils.Analyzer
{
    /// <summary>
    /// This class is the one that is inherited in mostly all def analyzers.
    /// </summary>
	public abstract class DefAnalyzer
	{
		public abstract bool Execute(object context);
	}
}