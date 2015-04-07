using Flame.Front.Cli;
using Flame.Front.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ProjectHandlers.RegisterHandler(new CFileHandler());
            var compiler = new ConsoleCompiler("cin", "Flame's C compiler", "https://github.com/jonathanvdc/cin/releases");
            compiler.Compile(args);
        }
    }
}
