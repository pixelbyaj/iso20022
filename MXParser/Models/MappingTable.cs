using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MXParser.Models
{
    public class MappingTable
    {
        public string NodeName { get; set; } = string.Empty;
        public string XPath { get; set; } = string.Empty;
        public IList<MappingColumn> Columns { get; set;} = new List<MappingColumn>();
    }
}
