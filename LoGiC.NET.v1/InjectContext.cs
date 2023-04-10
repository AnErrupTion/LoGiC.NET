using dnlib.DotNet;
using System.Collections.Generic;

namespace LoGiC.NET.Utils
{
    /// <summary>
    /// The context of an injection process.
    /// </summary>
    public class InjectContext
    {
        /// <summary>
        /// The mapping of origin definitions to injected definitions.
        /// </summary>
        public readonly Dictionary<IDnlibDef, IDnlibDef> Map = new Dictionary<IDnlibDef, IDnlibDef>();

        /// <summary>
        /// The module which source type originated from.
        /// </summary>
        public readonly ModuleDef OriginModule;

        /// <summary>
        /// The module which source type is being injected to.
        /// </summary>
        public readonly ModuleDef TargetModule;

        /// <summary>
        /// Initializes a new instance of the <see cref="InjectContext" /> class.
        /// </summary>
        /// <param name="module">The origin module.</param>
        /// <param name="target">The target module.</param>
        public InjectContext(ModuleDef module, ModuleDef target)
        {
            OriginModule = module;
            TargetModule = target;
            Importer = new Importer(target, ImporterOptions.TryToUseTypeDefs);
        }

        /// <summary>
        /// Gets the importer.
        /// </summary>
        /// <value>The importer.</value>
        public Importer Importer { get; }
    }
}
