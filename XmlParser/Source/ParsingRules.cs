using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using XmlParser.Models;

namespace XmlParser.Source
{
    public abstract class ParsingRules
    {
        const string NAMESPACE_PREFIX = "ns";
        internal IList<MappingRules>? _mappingRules;

        public abstract void DeserializeRules();

        internal async Task<DataSet?> ParseXmlRulesAsync(XmlDocument xmlDocument)
        {
            DataSet? dataSet = new();
            await Task.Run(() =>
            {
                XmlNamespaceManager namespaceManager = GetNamespaces(xmlDocument);

                if (xmlDocument != null && _mappingRules != null)
                {
                    foreach (MappingRules rule in _mappingRules)
                    {
                        dataSet = GetDefinedTable(rule.Mappings);
                        if (dataSet != null)
                        {
                            foreach (MappingTable row in rule.Mappings)
                            {
                                DataTable? table = dataSet.Tables[row.NodeName];
                                string tableXpath = string.Format("{0}{1}", "//", (GetXPath(row.XPath)));
                                XmlNode? tableNode = xmlDocument.SelectSingleNode(tableXpath, namespaceManager);
                                if (table != null && tableNode != null)
                                {
                                    DataRow dtRow = table.NewRow();
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
            });
            return dataSet;
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

        private static XmlNamespaceManager GetNamespaces(XmlDocument xmlDoc)
        {
            XmlNamespaceManager namespaces = new XmlNamespaceManager(xmlDoc.NameTable);

            // Select all namespace attributes in the document
            XmlNodeList? namespaceAttributes = xmlDoc.SelectNodes("//*[namespace-uri() != '']", namespaces);

            if (namespaceAttributes == null) 
                return namespaces;
            
            // Add each namespace to the dictionary
            foreach (XmlNode attribute in namespaceAttributes)
            {
                string uri = attribute.NamespaceURI;

                // Add the namespace to the dictionary
                namespaces.AddNamespace(NAMESPACE_PREFIX, uri);
            }

            return namespaces;
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


    }
}
