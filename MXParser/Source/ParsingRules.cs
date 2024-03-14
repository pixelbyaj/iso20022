using System.Data;
using System.Xml;

namespace MXParser
{
    public abstract class ParsingRules
    {
        #region const members
        const string COLUMN_MESSAGE_ID = "MessageId";
        const string NAMESPACE_PREFIX = "ns";
        const string NAMESPACE_APPHDR = "urn:iso:std:iso:20022:tech:xsd:head.001.001";
        #endregion

        #region private members
        private IList<MappingRules>? _mappingRules;
        #endregion

        #region public property
        public IList<MappingRules>? MapParsingRules
        {
            get
            {
                return _mappingRules;
            } 

            protected set
            {
                _mappingRules = value;
            }
        }
        #endregion

        #region private members
        private string _xmlNamespace = string.Empty;
        #endregion

        #region public methods
        public abstract void DeserializeRules();
        #endregion

        #region internal method
        internal async Task<DataSet?> ParseXmlRulesAsync(XmlDocument xmlDocument, Guid messageUniqueId)
        {
            DataSet? dataSet = new();
            await Task.Run(() =>
            {
                if (xmlDocument != null && _mappingRules != null)
                {
                    XmlNamespaceManager namespaceManager = GetNamespaces(xmlDocument);
                    var rules = _mappingRules.Where(c => _xmlNamespace.Contains(c.Namespace) || c.Namespace == NAMESPACE_APPHDR);
                    foreach (MappingRules rule in rules)
                    {
                        dataSet = GetDefinedTable(rule.Mappings);
                        if (dataSet != null)
                        {
                            foreach (MappingTable row in rule.Mappings)
                            {
                                DataTable? table = dataSet.Tables[row.NodeName];
                                string tableXpath = string.Format("{0}{1}", "//", (GetXPath(row.XPath)));
                                XmlNodeList? tableNodes = xmlDocument.SelectNodes(tableXpath, namespaceManager);
                                if (table != null && tableNodes != null)
                                {
                                    foreach (XmlNode tableNode in tableNodes)
                                    {
                                        DataRow dtRow = table.NewRow();
                                        dtRow[COLUMN_MESSAGE_ID] = messageUniqueId;
                                        foreach (MappingColumn col in row.Columns)
                                        {
                                            // Select the XML node using the XPath expression and namespace manager
                                            string xpath = string.Format("{0}{1}", "//", (GetXPath(col.XPath)));
                                            XmlNode? selectedNode = tableNode.SelectSingleNode(xpath, namespaceManager);
                                            dtRow[col.NodeName] = string.IsNullOrEmpty(selectedNode?.InnerText) ? DBNull.Value : selectedNode.InnerText;
                                        }
                                        table.Rows.Add(dtRow);
                                    }
                                }
                            }
                        }
                    }
                }
            });
            return dataSet;
        }
        #endregion

        #region private and private static members
        private XmlNamespaceManager GetNamespaces(XmlDocument xmlDoc)
        {
            XmlNamespaceManager namespaces = new(xmlDoc.NameTable);

            // Select all namespace attributes in the document
            XmlNode? namespaceAttributes = xmlDoc.SelectSingleNode("//*[namespace-uri() != '']", namespaces);
            XmlNode? namespaceDocAttributes = xmlDoc.SelectSingleNode("//*[local-name()='Document']", namespaces);
            XmlNode? namespaceAppHdrAttributes = xmlDoc.SelectSingleNode("//*[local-name()='AppHdr']", namespaces);

            if (namespaceDocAttributes != null)
            {
                _xmlNamespace = namespaceDocAttributes.NamespaceURI;
                namespaces.AddNamespace(NAMESPACE_PREFIX, _xmlNamespace);
            }

            if (namespaceAttributes != null)
            {
                namespaces.AddNamespace(NAMESPACE_PREFIX, namespaceAttributes.NamespaceURI);
            }

            if (namespaceAppHdrAttributes != null)
            {
                namespaces.AddNamespace(NAMESPACE_PREFIX, namespaceAppHdrAttributes.NamespaceURI);
            }



            return namespaces;
        }

        private static DataSet GetDefinedTable(IList<MappingTable> mappingTables)
        {
            if (mappingTables == null)
                return new();

            DataSet dataSet = new();
            foreach (MappingTable row in mappingTables)
            {
                DataTable table = new()
                {
                    TableName = row.NodeName
                };
                table.Columns.Add(COLUMN_MESSAGE_ID, typeof(Guid));
                foreach (MappingColumn col in row.Columns.OrderBy(c => c.OrderNumber))
                {
                    DataColumn dataColumn = new()
                    {
                        ColumnName = col.NodeName,
                        DataType = GetDataType(col.DataType)
                    };
                    table.Columns.Add(dataColumn);
                }
                dataSet.Tables.Add(table);
            }
            return dataSet;
        }

        private static string GetXPath(string xpath)
        {
            string[] xpathWithOR = xpath.Split("|");
            List<string> newXPath = new();
            for (int i = 0; i < xpathWithOR.Length; i++)
            {
                string[] nodes = xpathWithOR[i].TrimStart().TrimEnd().Split('/');
                for (int j = 0; j < nodes.Length; j++)
                {
                    if (nodes[j].StartsWith('@'))
                        continue;
                    nodes[j] = NAMESPACE_PREFIX + ":" + nodes[j];
                }
                string new_xpath = string.Join("/", nodes).Replace("[", string.Format("{0}{1}:", "[", NAMESPACE_PREFIX));
                newXPath.Add(new_xpath);

            }

            return string.Join(" | ", newXPath);
        }


        private static Type GetDataType(string name)
        {
            switch (name.ToLowerInvariant())
            {
                case "string":
                case "char":
                    return typeof(string);
                case "decimal":
                    return typeof(decimal);
                case "datetime":
                    return typeof(string);
                case "boolean":
                    return typeof(Boolean);
                // Add cases for other SQL data types as needed
                default:
                    throw new ArgumentException($"Unsupported SQL data type: {name}");

            }
        }

        #endregion

    }
}
