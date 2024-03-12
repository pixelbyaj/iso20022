using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using XmlParser.Models;

namespace XmlParser.Source
{
    public class ParsingJsonRules : ParsingRules
    {
        private string _jsonRules = string.Empty;
        public ParsingJsonRules(string jsonFilePath)
        {

            ArgumentNullException.ThrowIfNull(jsonFilePath, nameof(jsonFilePath));
            _jsonRules = File.ReadAllText(jsonFilePath);
            if (string.IsNullOrEmpty(_jsonRules))
            {
                throw new ArgumentNullException(nameof(jsonFilePath), "Empty JSON Rules File");
            }
            DeserializeRules();
        }

        public override void DeserializeRules()
        {
            _mappingRules = JsonConvert.DeserializeObject<List<MappingRules>>(_jsonRules);
        }
    }
}
