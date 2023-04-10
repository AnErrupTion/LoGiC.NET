using dnlib.DotNet;

namespace LoGiC.NET.Utils
{
    public static class MemberRenamer
    {
        // Thanks to the AsStrongAsFuck project!
        public static void GetRenamed(this IMemberDef member)
        {
            member.Name = Randomizer.String(StringLength());
        }

        public static int StringLength()
        {
            return Randomizer.Next(120, 30);
        }
    }
}
