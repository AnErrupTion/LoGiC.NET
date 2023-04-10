using System;
using System.IO;
using dnlib.DotNet;
using LoGiC.NET.Protections;
using SharpConfigParser;
using LoGiC.NET.Utils;
using dnlib.DotNet.Writer;

namespace LoGiC.NET
{
    class Program
    {
        public static ModuleDefMD Module { get; set; }

        public static string FileExtension { get; set; }

        public static bool DontRename { get; set; }

        public static bool ForceWinForms { get; set; }

        public static string FilePath { get; set; }

        public static MemoryStream Stream = new MemoryStream();

        static void Main(string[] _)
        {
            Console.WriteLine("- Drag & drop your file:");
            string path = Console.ReadLine().Replace("\"", string.Empty);

            Console.WriteLine("- Preparing obfuscation...");
            if (!File.Exists("config.txt"))
            {
                Console.WriteLine("Config file not found, continuing without it.");
                goto obfuscation;
            }

            Parser p = new Parser() { ConfigFile = "config.txt" };
            try { ForceWinForms = bool.Parse(p.Read("ForceWinFormsCompatibility").ReadResponse().ReplaceSpaces()); } catch { }
            try { DontRename = bool.Parse(p.Read("DontRename").ReadResponse().ReplaceSpaces()); } catch { }
            try { ProxyAdder.Intensity = int.Parse(p.Read("ProxyCallsIntensity").ReadResponse().ReplaceSpaces()); } catch { }

            Console.WriteLine("\n- ForceWinForms: " + ForceWinForms);
            Console.WriteLine("- DontRename: " + DontRename);
            Console.WriteLine("- ProxyCallsIntensity: " + ProxyAdder.Intensity + "\n");

            obfuscation:
            Module = ModuleDefMD.Load(path);
            FileExtension = Path.GetExtension(path);

            Protection[] protections = new Protection[]
            {
                new Renamer(),
                new AntiTamper(),
                new JunkDefs(),
                new StringEncryption(),
                new AntiDe4dot(),
                new ControlFlow(),
                new IntEncoding(),
                new ProxyAdder(),
                new InvalidMetadata()
            };

            foreach (Protection protection in protections)
            {
                Console.WriteLine("- Executing protection: " + protection.Name);
                protection.Execute();
            }

            Console.WriteLine("- Watermarking...");
            Watermark.AddAttribute();

            Console.WriteLine("- Saving file...");
            FilePath = @"C:\Users\" + Environment.UserName + @"\Desktop\" + Path.GetFileNameWithoutExtension(path) + "_protected" + FileExtension;
            Module.Write(Stream, new ModuleWriterOptions(Module) { Logger = DummyLogger.NoThrowInstance });

            Console.WriteLine("- Stripping DOS header...");
            StripDOSHeader.Execute();

            // Save stream to file
            File.WriteAllBytes(FilePath, Stream.ToArray());

            if (AntiTamper.Tampered)
                AntiTamper.Inject(FilePath);

            Console.WriteLine("- Done! Press any key to exit...");
            Console.ReadKey();
        }
    }
}
