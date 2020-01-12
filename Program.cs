using System;
using System.IO;
using dnlib.DotNet;
using dnlib.DotNet.Writer;
using LoGiC.NET.Protections;
using SharpConfigParser;
using LoGiC.NET.Utils;

namespace LoGiC.NET
{
    class Program
    {
        public static ModuleDefMD Module { get; set; }

        public static string FileExtension { get; set; }

        public static bool DontRename { get; set; }

        public static bool ForceWinForms { get; set; }

        public static string FilePath { get; set; }

        static void Main(string[] args)
        {
            Console.WriteLine("Drag & drop your file : ");
            string path = Console.ReadLine();

            Console.WriteLine("Preparing obfuscation...");
            if (!File.Exists("config.txt"))
            {
                Console.WriteLine("Config file not found, continuing without it.");
                goto obfuscation;
            }
            Parser p = new Parser() { ConfigFile = "config.txt" };
            ForceWinForms = bool.Parse(p.Read("ForceWinFormsCompatibility").ReadResponse().ReplaceSpaces());
            DontRename = bool.Parse(p.Read("DontRename").ReadResponse().ReplaceSpaces());

            obfuscation:
            Module = ModuleDefMD.Load(path);
            FileExtension = Path.GetExtension(path);

            Console.WriteLine("Renaming...");
            Renamer.Execute();

            Console.WriteLine("Adding junk methods...");
            JunkMethods.Execute();

            Console.WriteLine("Adding proxy calls...");
            ProxyAdder.Execute();

            Console.WriteLine("Encoding ints...");
            IntEncoding.Execute();

            Console.WriteLine("Encrypting strings...");
            StringEncryption.Execute();

            Console.WriteLine("Injecting Anti-Tamper...");
            AntiTamper.Execute();

            Console.WriteLine("Watermarking...");
            Watermark.AddAttribute();

            Console.WriteLine("Saving file...");
            FilePath = @"C:\Users\" + Environment.UserName + @"\Desktop\" + Path.GetFileNameWithoutExtension(path) + "_protected" +
                FileExtension;
            ModuleWriterOptions opts = new ModuleWriterOptions(Module) { Logger = DummyLogger.NoThrowInstance };
            Module.Write(FilePath, opts);

            if (AntiTamper.HasBeenTampered) AntiTamper.InjectMd5(FilePath);

            Console.WriteLine("Done! Press any key to exit...");
            Console.ReadKey();
        }
    }
}
