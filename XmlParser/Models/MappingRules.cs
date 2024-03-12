using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlParser.Models
{
    internal class MappingRules
    {
        public string Namespace { get; set; }
        public List<MappingTable> Mappings { get; set; }
        public MappingRules()
        {
            Namespace = string.Empty;
            Mappings = new();
        }
    }
}
