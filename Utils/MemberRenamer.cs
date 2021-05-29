using dnlib.DotNet;

namespace LoGiC.NET.Utils
{
    public static class MemberRenamer
    {
        // Thanks to the AsStrongAsFuck project!
        public static void GetRenamed(this IMemberDef member)
        {
            member.Name = Randomizer.Generated;
        }

        public static int StringLength()
        {
            return Randomizer.Next(10, 5);
        }
    }
}
