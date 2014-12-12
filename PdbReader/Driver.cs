﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="Driver.cs">
//   
// </copyright>
// <summary>
//   
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------
namespace PdbReader
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// </summary>
    public class Converter : IConverter
    {
        /// <summary>
        /// </summary>
        private readonly Dictionary<string, SourceFile> files = new Dictionary<string, SourceFile>();

        /// <summary>
        /// </summary>
        private readonly ISymbolWriter symbolWriter;

        /// <summary>
        /// </summary>
        private IDictionary<uint, PdbFunction> funcs = new SortedDictionary<uint, PdbFunction>();

        /// <summary>
        /// </summary>
        /// <param name="symbolWriter">
        /// </param>
        internal Converter(ISymbolWriter symbolWriter)
        {
            this.symbolWriter = symbolWriter;
        }

        /// <summary>
        /// </summary>
        /// <param name="filename">
        /// </param>
        /// <param name="symbolWriter">
        /// </param>
        internal Converter(string filename, ISymbolWriter symbolWriter)
        {
            this.symbolWriter = symbolWriter;
            using (var stream = File.OpenRead(filename))
            {
                foreach (var pdbFunc in PdbFile.LoadFunctions(stream, true))
                {
                    funcs[pdbFunc.token] = pdbFunc;
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="filename">
        /// </param>
        public static void Convert(string filename, ISymbolWriter symbolWriter)
        {
            using (var stream = File.OpenRead(filename))
            {
                var funcs = PdbFile.LoadFunctions(stream, true);
                Convert(funcs, symbolWriter);
            }
        }

        public static IConverter GetConverter(string filename, ISymbolWriter symbolWriter)
        {
            return new Converter(filename, symbolWriter);
        }

        public void ConvertFunction(int token)
        {
            PdbFunction func = null;
            if (this.funcs.TryGetValue((uint)token, out func))
            {
                this.ConvertFunction(func);
                return;
            }

            if (token == -1)
            {
                this.ConvertFunction(this.funcs.Values.First(f => f.lines != null), true);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="assembly">
        /// </param>
        /// <param name="functions">
        /// </param>
        /// <param name="symbolWriter">
        /// </param>
        internal static void Convert(IEnumerable<PdbFunction> functions, ISymbolWriter symbolWriter)
        {
            var converter = new Converter(symbolWriter);

            foreach (var function in functions)
            {
                converter.ConvertFunction(function);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="function">
        /// </param>
        /// <param name="generateFileOnly">
        /// </param>
        private void ConvertFunction(PdbFunction function, bool generateFileOnly = false)
        {
            if (function.lines == null)
            {
                return;
            }

            var method = new SourceMethod
            {
                Token = (int)function.token,
                Name = function.name,
                LinkageName = function.name,
                DisplayName = function.name,
                LineNumber = function.lines.First().lines.First().lineBegin
            };

            var file = this.GetSourceFile(this.symbolWriter, function);
            if (generateFileOnly)
            {
                return;
            }

            var builder = this.symbolWriter.OpenMethod(file.CompilationUnitEntry, method);

            this.ConvertSequencePoints(function, file, builder);

            this.ConvertVariables(function);

            this.symbolWriter.CloseMethod();
        }

        /// <summary>
        /// </summary>
        /// <param name="scope">
        /// </param>
        private void ConvertScope(PdbScope scope)
        {
            this.ConvertSlots(scope.slots);

            foreach (var s in scope.scopes)
            {
                this.ConvertScope(s);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="function">
        /// </param>
        /// <param name="file">
        /// </param>
        /// <param name="builder">
        /// </param>
        private void ConvertSequencePoints(PdbFunction function, SourceFile file, ISourceMethodBuilder builder)
        {
            var lastLine = 0;
            foreach (var line in function.lines.SelectMany(lines => lines.lines))
            {
                // 0xfeefee is an MS convention, we can't pass it into ISymbolWriter, so we use the last non-hidden line
                var isHidden = line.lineBegin == 0xfeefee;
                builder.MarkSequencePoint(
                    (int)line.offset, file, isHidden ? lastLine : (int)line.lineBegin, (int)line.colBegin, isHidden);
                if (!isHidden)
                {
                    lastLine = (int)line.lineBegin;
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="slots">
        /// </param>
        private void ConvertSlots(IEnumerable<PdbSlot> slots)
        {
            foreach (var slot in slots)
            {
                this.symbolWriter.DefineLocalVariable((int)slot.slot, slot.name);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="function">
        /// </param>
        private void ConvertVariables(PdbFunction function)
        {
            foreach (var scope in function.scopes)
            {
                this.ConvertScope(scope);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="symbolWriter">
        /// </param>
        /// <param name="function">
        /// </param>
        /// <returns>
        /// </returns>
        private SourceFile GetSourceFile(ISymbolWriter symbolWriter, PdbFunction function)
        {
            var name = (from l in function.lines where l.file != null select l.file.name).First();

            SourceFile file;
            if (this.files.TryGetValue(name, out file))
            {
                return file;
            }

            var entry = symbolWriter.DefineDocument(name);
            var unit = symbolWriter.DefineCompilationUnit(entry);

            file = new SourceFile(unit, entry);
            this.files.Add(name, file);
            return file;
        }

        /// <summary>
        /// </summary>
        private class SourceFile : ISourceFile
        {
            /// <summary>
            /// </summary>
            private readonly ICompileUnitEntry compileUnitEntry;

            /// <summary>
            /// </summary>
            private readonly ISourceFileEntry entry;

            /// <summary>
            /// </summary>
            /// <param name="compileUnitEntry">
            /// </param>
            /// <param name="entry">
            /// </param>
            public SourceFile(ICompileUnitEntry compileUnitEntry, ISourceFileEntry entry)
            {
                this.compileUnitEntry = compileUnitEntry;
                this.entry = entry;
            }

            /// <summary>
            /// </summary>
            public ICompileUnitEntry CompilationUnitEntry
            {
                get
                {
                    return this.compileUnitEntry;
                }
            }

            /// <summary>
            /// </summary>
            public ISourceFileEntry Entry
            {
                get
                {
                    return this.entry;
                }
            }
        }

        /// <summary>
        /// </summary>
        private class SourceMethod : ISourceMethod
        {
            /// <summary>
            /// </summary>
            public int Token { get; set; }

            public string Name { get; set; }

            public string DisplayName { get; set; }

            public string LinkageName { get; set; }

            public uint LineNumber { get; set; }
        }
    }
}