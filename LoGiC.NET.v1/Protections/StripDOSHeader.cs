using System.IO;

namespace LoGiC.NET.Protections
{
    public class StripDOSHeader
    {
        private static readonly uint offset_lfanew = 0x3C;
        private static readonly int length_lfanew = sizeof(uint);

        private static readonly uint offset_magic = 0x00;
        private static readonly int length_magic = sizeof(ushort);

        public static void Execute()
        {
            byte[] blank_dos = new byte[64];
            byte[] magic = ReadArray(offset_magic, length_magic, Program.Stream);
            byte[] lfanew = ReadArray(offset_lfanew, length_lfanew, Program.Stream);

            Program.Stream.Position = 0;
            WriteArray(0, blank_dos, Program.Stream);
            WriteArray(offset_magic, magic, Program.Stream);
            WriteArray(offset_lfanew, lfanew, Program.Stream);
            WriteArray(0x4e, new byte[39], Program.Stream); // Overrides "This program can not be run in DOS mode."
        }

        private static byte[] ReadArray(uint offset, int length, Stream stream)
        {
            var data = new byte[length];
            stream.Position = offset;
            stream.Read(data, 0, data.Length);
            return data;
        }

        private static int WriteArray(uint offset, byte[] data, Stream stream)
        {
            stream.Position = offset;
            stream.Write(data, 0, data.Length);
            return data.Length;
        }
    }
}
