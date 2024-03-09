using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlParser.Models
{
    internal class MappingColumn
    {
        public int OrderNumber { get; set; }
        public string NodeName { get; set; } = string.Empty;
        public string XPath { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public string MinLength { get; set; }

        public int MaxLength { get; set; }
        public int fractionDigits { get; set; }
        public int totalDigits { get; set; }
    }
}
