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

        /// <summary>
        /// Resolves a choosed TypeDef from the Map.
        /// </summary>
        /// <param name="typeDef">The TypeDef that will be searched.</param>
        /// <returns>A TypeDef from the Map or null if it cannot find it.</returns>
        public TypeDef Resolve(TypeDef typeDef)
        {
            if (Map.ContainsKey(typeDef)) return (TypeDef)Map[typeDef];
            return null;
        }

        /// <summary>
        /// Resolves a choosed MethodDef from the Map.
        /// </summary>
        /// <param name="methodDef">The MethodDef that will be searched.</param>
        /// <returns>A MethodDef from the Map or null if it cannot find it.</returns>
        public MethodDef Resolve(MethodDef methodDef)
        {
            if (Map.ContainsKey(methodDef)) return (MethodDef)Map[methodDef];
            return null;
        }

        /// <summary>
        /// Resolves a choosed FieldDef from the Map.
        /// </summary>
        /// <param name="fieldDef">The FieldDef that will be searched.</param>
        /// <returns>A FieldDef from the Map or null if it cannot find it.</returns>
        public FieldDef Resolve(FieldDef fieldDef)
        {
            if (Map.ContainsKey(fieldDef)) return (FieldDef)Map[fieldDef];
            return null;
        }
    }
}
