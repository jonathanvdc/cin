using Flame.Compiler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cin
{
    public class LazySourceDocument : ISourceDocument
    {
        public LazySourceDocument(string Identifier, ICompilerLog Log)
        {
            this.Identifier = Identifier;
            this.Log = Log;
        }

        public string Identifier { get; private set; }
        public ICompilerLog Log { get; private set; }

        private ISourceDocument doc;
        public ISourceDocument Document
        {
            get
            {
                if (doc == null)
                {
                    doc = GetSourceSafe();
                }
                return doc;
            }
        }

        private ISourceDocument GetSourceSafe()
        {
            try
            {
                using (var stream = new FileStream(Identifier, FileMode.Open))
                using (var reader = new StreamReader(stream))
                {
                    string code = reader.ReadToEnd();
                    return new SourceDocument(code, Identifier);
                }
            }
            catch (FileNotFoundException ex)
            {
                Log.LogError(new LogEntry("Error getting source code", "File '" + Identifier + "' was not found"));
                return null;
            }
            catch (Exception ex)
            {
                Log.LogError(new LogEntry("Error getting source code", "'" + Identifier + "' could not be opened"));
                Log.LogError(new LogEntry("Exception", ex.ToString()));
                return null;
            }
        }

        public string GetLine(int Index)
        {
            return Document.GetLine(Index);
        }

        public int LineCount
        {
            get { return Document.LineCount; }
        }

        public string Source
        {
            get { return Document.Source; }
        }

        public SourceGridPosition ToGridPosition(int CharacterIndex)
        {
            return Document.ToGridPosition(CharacterIndex);
        }
    }
}
