﻿using Flame;
using Flame.Build;
using Flame.C.Build;
using Flame.C.Lexer;
using Flame.C.Parser;
using Flame.C.Preprocessor;
using Flame.Compiler;
using Flame.Compiler.Projects;
using Flame.Front;
using Flame.Front.Options;
using Flame.Front.Projects;
using Flame.Syntax;
using Flame.Syntax.C;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flame.Front.Target;
using Flame.Verification;

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
			return new SingleFileProject(Path, Log.Options.GetTargetPlatform());
        }

		public IProject MakeProject(IProject Project, ProjectPath Path, ICompilerLog Log)
		{
			Log.LogWarning(new LogEntry(
				"ignored '-make-project'",
				"the '-make-project' option was ignored because cin does not support any C project formats yet."));
			return Project;
		}

		public IEnumerable<ParsedProject> Partition(IEnumerable<ParsedProject> Projects)
		{
			return new ParsedProject[] { new ParsedProject(Projects.First().CurrentPath, UnionProject.CreateUnion(Projects.Select(item => item.Project).ToArray())) };
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
                units[i] = ParseCompilationUnitAsync(item, Assembly, Parameters);
            }
            return Task.WhenAll(units);
        }
        public static ISourceDocument GetSourceSafe(IProjectSourceItem Item, CompilationParameters Parameters)
        {
            try
            {
                return Item.GetSource(Parameters.CurrentPath.AbsolutePath.Path);
            }
            catch (FileNotFoundException)
            {
                Parameters.Log.LogError(new LogEntry("error getting source code", "file '" + Item.SourceIdentifier + "' was not found."));
                return null;
            }
            catch (Exception ex)
            {
                Parameters.Log.LogError(new LogEntry("error getting source code", "'" + Item.SourceIdentifier + "' could not be opened."));
                Parameters.Log.LogError(new LogEntry("exception", ex.ToString()));
                return null;
            }
        }

        public static TokenizerStream Preprocess(ISourceDocument Document, CompilationParameters Parameters)
        {
            var sourceFile = new SourceFile(Document, Parameters);
            var preprocessor = new PreprocessorState(PreprocessorEnvironment.Static_Singleton.Instance.CreateDefaultEnvironment(Parameters.Log), sourceFile);
            var result = preprocessor.Expand(Document);
            if (Parameters.Log.Options.GetOption<bool>("output-preprocessed", false) || Parameters.Log.Options.GetOption<bool>("E", false))
            {
                Parameters.Log.LogMessage(new LogEntry(Document.Identifier + " after preprocessing", result.ToString()));
            }
            return new TokenizerStream(result.ToStream());
        }

        public static Task<CompilationUnit> ParseCompilationUnitAsync(IProjectSourceItem SourceItem, SyntaxAssembly Assembly, CompilationParameters Parameters)
        {
            Parameters.Log.LogEvent(new LogEntry("Status", "parsing " + SourceItem.SourceIdentifier));
            return Task.Run(() =>
            {
                var code = GetSourceSafe(SourceItem, Parameters);
                if (code == null)
                {
                    return null;
                }
                var parser = Preprocess(code, Parameters);
                var unit = ParseCompilationUnit(parser, Assembly);
                Parameters.Log.LogEvent(new LogEntry("Status", "parsed " + SourceItem.SourceIdentifier));
                return unit;
            });
        }
        public static CompilationUnit ParseCompilationUnit(ITokenStream TokenParser, SyntaxAssembly Assembly)
        {
            var syntaxParser = new CSyntaxParser(Assembly.Log);
            var unit = Assembly.CreateCompilationUnit();
            var state = unit.GetSyntaxState();

            IDeclarationSyntax[] decls;
            try
            {
                decls = syntaxParser.ParseAllDeclarations(TokenParser).ToArray();
            }
            catch (Exception ex)
            {
                Assembly.Log.LogError(new LogEntry(
					"error parsing source", 
					"an error occurred while parsing source code.", 
					TokenParser.PeekNoTrivia(TokenParser.CurrentPosition).TokenPeek.FullLocation));
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
                    Assembly.Log.LogError(new LogEntry(
						"error applying declaration", 
						"an error occurred while applying a declaration.", 
						item.GetSourceLocation()));
                    Assembly.Log.LogException(ex);
                    throw;
                }
            }

            return unit;
        }

		public PassPreferences GetPassPreferences(ICompilerLog Log)
		{
			return new PassPreferences(new PassCondition[]
			{
				new PassCondition(
					PassExtensions.EliminateDeadCodePassName,
					optInfo => optInfo.OptimizeMinimal || optInfo.OptimizeDebug),
				new PassCondition(
					InfiniteRecursionPass.InfiniteRecursionPassName,
					optInfo => InfiniteRecursionPass.IsUseful(optInfo.Log))
			},
				new PassInfo<Tuple<IStatement, IMethod, ICompilerLog>, IStatement>[]
			{
				new PassInfo<Tuple<IStatement, IMethod, ICompilerLog>, IStatement>(
					VerifyingDeadCodePass.Instance,
					PassExtensions.EliminateDeadCodePassName),
				
				new PassInfo<Tuple<IStatement, IMethod, ICompilerLog>, IStatement>(
					InfiniteRecursionPass.Instance,
					InfiniteRecursionPass.InfiniteRecursionPassName)
			});
		}
    }
}
