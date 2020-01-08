using System;
using System.IO;
using System.Security.Cryptography;
using System.Reflection;

namespace LoGiC.NET.Utils
{
    class TamperClass
    {
        static void NoTampering()
        {
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            Stream stream = new StreamReader(assemblyLocation).BaseStream;
            BinaryReader reader = new BinaryReader(stream);
            string newMd5 = BitConverter.ToString(MD5.Create().ComputeHash(reader.ReadBytes(File.ReadAllBytes(assemblyLocation).Length -
                16)));
            stream.Seek(-16, SeekOrigin.End);
            string realMd5 = BitConverter.ToString(reader.ReadBytes(16));
            if (newMd5 != realMd5) throw new EntryPointNotFoundException();
        }
    }
}
