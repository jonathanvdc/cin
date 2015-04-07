﻿using Flame;
using Flame.Build;
using Flame.C.Build;
using Flame.C.Lexer;
using Flame.C.Parser;
using Flame.Compiler;
using Flame.Compiler.Projects;
using Flame.Front.Projects;
using Flame.Syntax;
using Flame.Syntax.C;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cin
{
    public class CFileHandler : IProjectHandler
    {
        public async Task<IAssembly> CompileAsync(IProject Project, CompilationParameters Parameters)
        {
            var binder = await Parameters.BinderTask;
            var asm = new SyntaxAssembly(Project.Name, new Version(), Parameters.Log,
                CBuildHelpers.Instance.CreatePrimitiveBinder32(binder), new TypeNamerBase());

            await ParseCompilationUnitsAsync(Project.GetSourceItems(), asm, Parameters);

            return asm;
        }

        public IEnumerable<string> Extensions
        {
            get { return new string[] { "c" }; }
        }

        public IProject Parse(ProjectPath Path, ICompilerLog Log)
        {
            return new SingleFileProject(Path);
        }

        private static IConverter<IType, string> GetTypeNamer(ICompilerOptions Options)
        {
            switch (Options.GetOption<string>("type-names", "default"))
            {
                case "trivial":
                case "prefer-trivial":
                case "precise":
                case "default":
                default:
                    return new TypeNamerBase();
            }
        }

        public static Task ParseCompilationUnitsAsync(List<IProjectSourceItem> SourceItems, SyntaxAssembly Assembly, CompilationParameters Parameters)
        {
            Task[] units = new Task[SourceItems.Count];
            for (int i = 0; i < units.Length; i++)
            {
                var item = SourceItems[i];
                units[i] = ParseCompilationUnitAsync(item, Assembly, Parameters)
                    .ContinueWith(async result => Assembly.AddCompilationUnit(await result));
            }
            return Task.WhenAll(units);
        }
        public static ISourceDocument GetSourceSafe(IProjectSourceItem Item, CompilationParameters Parameters)
        {
            try
            {
                return Item.GetSource(Parameters.CurrentPath.AbsolutePath.Path);
            }
            catch (FileNotFoundException ex)
            {
                Parameters.Log.LogError(new LogEntry("Error getting source code", "File '" + Item.SourceIdentifier + "' was not found"));
                return null;
            }
            catch (Exception ex)
            {
                Parameters.Log.LogError(new LogEntry("Error getting source code", "'" + Item.SourceIdentifier + "' could not be opened"));
                Parameters.Log.LogError(new LogEntry("Exception", ex.ToString()));
                return null;
            }
        }

        public static Task<CompilationUnit> ParseCompilationUnitAsync(IProjectSourceItem SourceItem, SyntaxAssembly Assembly, CompilationParameters Parameters)
        {
            Parameters.Log.LogEvent(new LogEntry("Status", "Parsing " + SourceItem.SourceIdentifier));
            return Task.Run(() =>
            {
                var code = GetSourceSafe(SourceItem, Parameters);
                if (code == null)
                {
                    return null;
                }
                var parser = new TokenizerStream(code);
                var unit = ParseCompilationUnit(parser, Assembly);
                Parameters.Log.LogEvent(new LogEntry("Status", "Parsed " + SourceItem.SourceIdentifier));
                return unit;
            });
        }
        public static CompilationUnit ParseCompilationUnit(ITokenStream TokenParser, SyntaxAssembly Assembly)
        {
            var syntaxParser = new CSyntaxParser(Assembly.Log);
            var unit = new CompilationUnit(Assembly);
            var state = unit.GetSyntaxState();

            IDeclarationSyntax[] decls;
            try
            {
                decls = syntaxParser.ParseAllDeclarations(TokenParser).ToArray();
            }
            catch (Exception ex)
            {
                Assembly.Log.LogError(new LogEntry("Error parsing source", "An error occurred while parsing source code.", TokenParser.PeekNoTrivia(TokenParser.CurrentPosition).TokenPeek.FullLocation));
                Assembly.Log.LogException(ex);
                throw;
            }


            foreach (var item in decls)
            {
                try
                {
                    item.Declare(state);
                }
                catch (Exception ex)
                {
                    Assembly.Log.LogError(new LogEntry("Error applying declaration", "An error occurred while applying a declaration.", item.GetSourceLocation()));
                    Assembly.Log.LogException(ex);
                    throw;
                }
            }

            return unit;
        }
    }
}