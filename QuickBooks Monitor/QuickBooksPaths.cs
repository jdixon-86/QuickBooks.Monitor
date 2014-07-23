using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickBooks_Monitor
{
    public class QuickBooksPaths
    {
        public string Path { get; set; }

        public string Filename { get; set; }

        public string LookFor { get; set; }

        public string ReplaceWith { get; set; }

        public string FullPath
        {
            get
            {
                return string.Format("{0}/{1}", Path, Filename);
            }
        }
    }
}
