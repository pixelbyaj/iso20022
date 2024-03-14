using System.Data;
using System.Xml;

namespace MXParser
{
    sealed public class ParsingService : IParsingService
    {
        #region private members
        private readonly string _documentRootNodeName;
        private readonly ParsingRules _parsingRules;
        #endregion

        #region ctor
        public ParsingService(ParsingRules parsingRules, string documentRootNodeName = "Document")
        {
            ArgumentNullException.ThrowIfNull(typeof(ParsingRules));
            _documentRootNodeName = documentRootNodeName;
            _parsingRules = parsingRules;
        }
        #endregion

        #region public methods
        /// <summary>
        /// Get list of dataset of a single parsed XML
        /// </summary>
        /// <param name="streamReader"></param>
        /// <returns></returns>
        public async Task<IList<DataSet>> ParseXmlAsync(StreamReader streamReader)
        {
            using XmlReader reader = XmlReader.Create(streamReader, new XmlReaderSettings { Async = true, ConformanceLevel = ConformanceLevel.Auto });
            XmlDocument childDoc = childDoc = new();
            List<DataSet> dataSets = new();
            Guid messageUniqueId = Guid.NewGuid();
            while (await reader.ReadAsync())
            {
               if (reader.NodeType == XmlNodeType.EndElement && reader.Name == _documentRootNodeName)
                {
                    childDoc = new XmlDocument();
                    childDoc.Load(reader.ReadSubtree());
                    DataSet? data = await ApplyParsingRulesAsync(childDoc, messageUniqueId);
                    if (data != null)
                    {
                        dataSets.Add(data);
                    }
                }
            }
            return dataSets;
        }
       
        public async Task ParseXmlAsync(StreamReader streamReader, Func<DataSet,Guid, Guid, Task> callback)
        {
            using XmlReader reader = XmlReader.Create(streamReader, new XmlReaderSettings { Async = true, ConformanceLevel = ConformanceLevel.Auto });
            XmlDocument childDoc = childDoc = new();
            List<DataSet> dataSets = new();
            Guid messageUniqueId = Guid.NewGuid();
            while (await reader.ReadAsync())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == _documentRootNodeName)
                {
                    childDoc = new XmlDocument();
                    childDoc.Load(reader.ReadSubtree());
                    DataSet? data = await ApplyParsingRulesAsync(childDoc, messageUniqueId);
                    if (data != null)
                    {
                        if(callback != null)
                        await callback(data, Guid.NewGuid(), messageUniqueId);
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

        public async Task ParseXmlAsync(string filePath, Func<DataSet,Guid, Guid, Task> callback)
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
        #endregion

        #region private static methods
        private async Task<DataSet?> ApplyParsingRulesAsync(XmlDocument xmlDocument, Guid messageUniqueId)
        {
            return await _parsingRules.ParseXmlRulesAsync(xmlDocument, messageUniqueId);
        }
        #endregion

    }
}
