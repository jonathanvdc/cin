using Flame.C.Preprocessor;
using Flame.Compiler;
using Flame.Front.Projects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cin
{
    public class SourceFile : ISourceFile
    {
        public SourceFile(ISourceDocument Document, CompilationParameters Parameters)
        {
            this.Document = Document;
            this.Parameters = Parameters;
        }

        public ISourceDocument Document { get; private set; }
        public CompilationParameters Parameters { get; private set; }

        public ISourceDocument GetDocument()
        {
            return Document;
        }       

        public IIncludeConfiguration IncludeConfiguration
        {
            get { return new IncludeConfiguration(Path.GetDirectoryName(Path.GetFullPath(Document.Identifier)), Parameters); }
        }
        
        public string Identifier
        {
            get { return Document.Identifier; }
        }
    }
}
