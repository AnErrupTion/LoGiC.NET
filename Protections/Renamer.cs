using System;
using dnlib.DotNet;
using LoGiC.NET.Utils;
using LoGiC.NET.Utils.Analyzer;

namespace LoGiC.NET.Protections
{
    public class Renamer : Randomizer
    {
        private static int TypeAmount { get; set; }

        private static int MethodAmount { get; set; }

        private static int ParameterAmount { get; set; }

        private static int PropertyAmount { get; set; }

        private static int FieldAmount { get; set; }

        private static int EventAmount { get; set; }

        /// <summary>
        /// Execution of the 'Renamer' method. It'll rename types, methods and their parameters, properties, fields and events to random strings. But before they get renamed, they get analyzed to see if they are good to be renamed. (That prevents the program being broken)
        /// </summary>
        public static void Execute()
        {
            if (Program.DontRename) return;

            Program.Module.Mvid = Guid.NewGuid();
            Program.Module.Name = GenerateRandomString(Next(700, 500));
            Program.Module.EntryPoint.Name = GenerateRandomString(Next(700, 500));

            foreach (TypeDef type in Program.Module.Types)
            {
                if (CanRename(type) && !Program.FileExtension.Contains("dll") && !Program.ForceWinForms && !Program.Module.HasResources)
                {
                    type.Name = GenerateRandomString(Next(700, 500));
                    type.Namespace = GenerateRandomString(Next(700, 500));
                    ++TypeAmount;
                }

                foreach (MethodDef m in type.Methods)
                {
                    if (CanRename(m) && !Program.ForceWinForms && !Program.FileExtension.Contains("dll"))
                    {
                        m.Name = GenerateRandomString(Next(700, 500));
                        ++MethodAmount;
                    }

                    foreach (Parameter para in m.Parameters)
                    {
                        if (CanRename(para))
                        {
                            para.Name = GenerateRandomString(Next(700, 500));
                            ++ParameterAmount;
                        }
                    }
                }

                foreach (PropertyDef p in type.Properties)
                {
                    if (CanRename(p))
                    {
                        p.Name = GenerateRandomString(Next(700, 500));
                        ++PropertyAmount;
                    }
                }

                foreach (FieldDef field in type.Fields)
                {
                    if (CanRename(field))
                    {
                        field.Name = GenerateRandomString(Next(700, 500));
                        ++FieldAmount;
                    }
                }

                foreach (EventDef e in type.Events)
                {
                    if (CanRename(e))
                    {
                        e.Name = GenerateRandomString(Next(700, 500));
                        ++EventAmount;
                    }
                }
            }

            Console.WriteLine($"Renamed {TypeAmount} types.\nRenamed {MethodAmount} methods.\nRenamed {ParameterAmount} parameters.\n" +
                $"Renamed {PropertyAmount} properties.\nRenamed {FieldAmount} fields.\nRenamed {EventAmount} events.");
        }

        /// <summary>
        /// This will check with some Analyzers if it's possible to rename a determinate { TypeDef, MethodDef, EventDef, FieldDef }.
        /// </summary>
        /// <param name="obj">The determinate to check.</param>
        /// <returns>If the determinate can be renamed.</returns>
		public static bool CanRename(object obj)
        {
            DefAnalyzer analyze = null;
            if (obj is TypeDef)
                analyze = new TypeDefAnalyzer();
            else if (obj is MethodDef)
                analyze = new MethodDefAnalyzer();
            else if (obj is EventDef)
                analyze = new EventDefAnalyzer();
            else if (obj is FieldDef)
                analyze = new FieldDefAnalyzer();
            else if (obj is Parameter)
                analyze = new ParameterAnalyzer();
            if (analyze == null)
                return false;
            return analyze.Execute(obj);
        }
    }
}
