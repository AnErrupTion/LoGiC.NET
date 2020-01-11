using dnlib.DotNet;

namespace LoGiC.NET.Utils
{
    public static class MemberRenamer
    {
        // Thanks to the AsStrongAsFuck project!
        public static void Rename(this IMemberDef member, string name)
        {
            member.Name = name;
        }
    }
}
