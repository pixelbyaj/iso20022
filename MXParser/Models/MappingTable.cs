namespace MXParser
{
    public class MappingTable
    {
        public string NodeName { get; set; } = string.Empty;
        public string XPath { get; set; } = string.Empty;
        public IList<MappingColumn> Columns { get; set;} = new List<MappingColumn>();
    }
}
