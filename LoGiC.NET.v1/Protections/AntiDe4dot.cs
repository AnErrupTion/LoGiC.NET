using dnlib.DotNet;

namespace LoGiC.NET.Protections
{
    public class AntiDe4dot : Protection
    {
        public AntiDe4dot()
        {
            Name = "Anti-De4dot";
        }

        public override void Execute()
        {
            foreach (ModuleDef module in Program.Module.Assembly.Modules)
            {
                InterfaceImplUser int1 = new InterfaceImplUser(module.GlobalType);
                for (int i = 0; i < 1; i++)
                {
                    TypeDefUser typeDef1 = new TypeDefUser(string.Empty, $"Form{i}", module.CorLibTypes.GetTypeRef("System", "Attribute"));
                    InterfaceImplUser int2 = new InterfaceImplUser(typeDef1);

                    module.Types.Add(typeDef1);

                    typeDef1.Interfaces.Add(int2);
                    typeDef1.Interfaces.Add(int1);
                }
            }
        }
    }
}
