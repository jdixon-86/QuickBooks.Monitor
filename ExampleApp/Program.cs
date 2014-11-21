using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileStateEnforcer;

namespace ExampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var lineRule = new LineRule(
                directory: Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                filename: "example_enforcer.ini",
                search: "example line=",
                rule: "example line=chicken");

            Console.WriteLine("Automatic? (Y/N)");
            if (!Console.ReadLine().ToUpper().StartsWith("Y"))
            {
                Console.WriteLine("Please enter directory to watch.");
                var dir = Console.ReadLine();

                Console.WriteLine("Please enter filename.");
                var filename = Console.ReadLine();

                Console.WriteLine("Please enter line to enforce.");
                var line = Console.ReadLine();

                Console.WriteLine("Please enter rule.");
                var rule = Console.ReadLine();

                lineRule = new LineRule(dir, filename, line, rule);
            }

            File.WriteAllText(lineRule.FullPath, lineRule.Rule + Environment.NewLine);

            using (var enforcer = new Enforcer(lineRule))
            {
                enforcer.Begin(() => Console.WriteLine("Changed!"));

                Console.WriteLine("Watching...");
                Console.ReadKey();

                Console.WriteLine("Finished...");
            }
        }
    }
}
