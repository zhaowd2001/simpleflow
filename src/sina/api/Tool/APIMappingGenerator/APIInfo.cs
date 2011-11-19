using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace APIMappingGenerator
{
    [DebuggerDisplay("{APIMethodSignature}, {APIUri}")]
    public class APIInfo
    {
        public string APIMethodName { get; set; }

        public string APIMethodSignature { get; set; }

        public string APIUri { get; set; }

        public string WikiUri { get; set; }

        public string Summary { get; set; }
    }
}
