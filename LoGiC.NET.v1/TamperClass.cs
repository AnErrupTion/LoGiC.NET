using System;
using System.IO;
using System.Security.Cryptography;
using System.Reflection;

namespace LoGiC.NET.Utils
{
    public static class TamperClass
    {
        public static void NoTampering()
        {
            string p = Assembly.GetExecutingAssembly().Location;
            Stream l = new StreamReader(p).BaseStream;
            BinaryReader r = new BinaryReader(l);
            string g = BitConverter.ToString(MD5.Create().ComputeHash(r.ReadBytes(File.ReadAllBytes(p).Length - 16)));
            l.Seek(-16, SeekOrigin.End);
            if (g != BitConverter.ToString(r.ReadBytes(16))) throw new EntryPointNotFoundException();
        }
    }
}
