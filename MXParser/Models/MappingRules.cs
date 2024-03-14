namespace MXParser
{
    public class MappingRules
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
