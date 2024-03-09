using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using XmlParser.Models;
using XmlParser.Source;

namespace XmlParser.Services
{
    public interface IParsingService
    {
        public Task<IList<DataSet>> ParseXmlAsync(string filePath);
        public Task ParseXmlAsync(StreamReader streamReader, Func<DataSet, Task> callback);
        public Task<IList<DataSet>> ParseXmlAsync(StreamReader streamReader);
        public Task ParseXmlAsync(string filePath, Func<DataSet, Task> callback);
    }
}
