using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FileStateEnforcer
{
    public class LineRule
    {
        public LineRule(string directory, string filename, string search, string rule)
        {
            Directory = directory;
            Filename = filename;
            Search = search;
            Rule = rule;
        }

        public string Directory { get; private set; }
        public string Filename { get; private set; }
        public string Search { get; private set; }
        public string Rule { get; private set; }

        public string FullPath
        {
            get
            {
                return Path.Combine(Directory, Filename);
            }
        }
    }
}
