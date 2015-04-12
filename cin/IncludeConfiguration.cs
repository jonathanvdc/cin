using Flame.C.Preprocessor;
using Flame.Front;
using Flame.Front.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cin
{
    public class IncludeConfiguration : IIncludeConfiguration
    {
        public IncludeConfiguration(string DirectoryPath, CompilationParameters Parameters)
        {
            this.DirectoryPath = DirectoryPath;
            this.Parameters = Parameters;
        }

        public string DirectoryPath { get; private set; }
        public CompilationParameters Parameters { get; private set; }

        public ISourceFile GetAdditionalIncludeFile(string Path)
        {
            if (Parameters.Log.Options.HasOption("includes"))
            {
                foreach (var includeDir in Parameters.Log.Options.GetOption<string[]>("includes", new string[] { }))
                {
                    string absPath = new PathIdentifier(includeDir, Path).AbsolutePath.Path;
                    if (System.IO.File.Exists(absPath))
                    {
                        return new SourceFile(new LazySourceDocument(absPath, Parameters.Log), Parameters);
                    }
                }
            }
            return null;
        }

        public ISourceFile GetLocalIncludeFile(string Path)
        {
            string absPath = new PathIdentifier(DirectoryPath, Path).AbsolutePath.Path;
            if (System.IO.File.Exists(absPath))
            {
                return new SourceFile(new LazySourceDocument(absPath, Parameters.Log), Parameters);
            }
            return null;
        }

        public ISourceFile GetStandardIncludeFile(string Path)
        {
            return null;
        }
    }
}
