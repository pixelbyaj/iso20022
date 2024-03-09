using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using XmlParser.Models;
using XmlParser.Source;

namespace XmlParser.Services
{
    public class ParsingService : IParsingService
    {
        private readonly string _rootNodeName;
        private readonly ParsingRules _parsingRules;

        public ParsingService(ParsingRules parsingRules, string rootNodeName = "Document")
        {
            ArgumentNullException.ThrowIfNull(typeof(ParsingRules));
            _rootNodeName = rootNodeName;
            _parsingRules = parsingRules;
        }

        public async Task<IList<DataSet>> ParseXmlAsync(StreamReader streamReader)
        {
            using XmlReader reader = XmlReader.Create(streamReader, new XmlReaderSettings { Async = true, ConformanceLevel = ConformanceLevel.Auto });
            XmlDocument childDoc = childDoc = new();
            List<DataSet> dataSets = new();
            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == _rootNodeName)
                {
                    childDoc = new XmlDocument();
                    childDoc.Load(reader.ReadSubtree());
                    DataSet? data = await ApplyParsingRulesAsync(childDoc);
                    if (data != null)
                    {
                        dataSets.Add(data);
                    }
                }
            }
            return dataSets;
        }
        public async Task ParseXmlAsync(StreamReader streamReader, Func<DataSet, Task> callback)
        {
            using XmlReader reader = XmlReader.Create(streamReader, new XmlReaderSettings { Async = true, ConformanceLevel = ConformanceLevel.Auto });
            XmlDocument childDoc = childDoc = new();
            List<DataSet> dataSets = new();
            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == _rootNodeName)
                {
                    childDoc = new XmlDocument();
                    childDoc.Load(reader.ReadSubtree());
                    DataSet? data = await ApplyParsingRulesAsync(childDoc);
                    if (data != null)
                    {
                        if(callback != null)
                        await callback(data);
                    }
                }
            }
        }

        public async Task<IList<DataSet>> ParseXmlAsync(string filePath)
        {
            ArgumentNullException.ThrowIfNull(typeof(string), filePath);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found", filePath);
            };

            StreamReader? reader = new(filePath);

            ArgumentNullException.ThrowIfNull(typeof(StreamReader), filePath);
            return await ParseXmlAsync(reader);
        }

        public async Task ParseXmlAsync(string filePath, Func<DataSet, Task> callback)
        {
            ArgumentNullException.ThrowIfNull(typeof(string), filePath);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found", filePath);
            };

            StreamReader? reader = new(filePath);

            ArgumentNullException.ThrowIfNull(typeof(StreamReader), filePath);
            await ParseXmlAsync(reader, callback);
        }

        private async Task<DataSet?> ApplyParsingRulesAsync(XmlDocument xmlDocument)
        {
            return await _parsingRules.ParseXmlRulesAsync(xmlDocument);
        }
    }
}
