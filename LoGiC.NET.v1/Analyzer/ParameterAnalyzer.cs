using dnlib.DotNet;

namespace LoGiC.NET.Utils.Analyzer
{
    /// <summary>
    /// This class will analyze a function parameter.
    /// </summary>
    public class ParameterAnalyzer : DefAnalyzer
    {
        public override bool Execute(object context)
        {
            return ((Parameter)context).Name != string.Empty;
        }
    }
}
