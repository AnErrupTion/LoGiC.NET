using dnlib.DotNet;

namespace LoGiC.NET.Utils.Analyzer
{
    /// <summary>
    /// This class will analyze a parameter (NOT def).
    /// </summary>
    public class ParameterAnalyzer : DefAnalyzer
    {
        public override bool Execute(object context)
        {
            Parameter param = (Parameter)context;
            if (param.Name == string.Empty)
                return false;
            return true;
        }
    }
}
