using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MXParser
{
    public interface IParsingService
    {
   
        /// <summary>
        /// Parse xml by given streamReader
        /// </summary>
        /// <param name="streamReader"></param>
        /// <returns>List of DataSet</returns>
        public Task<IList<DataSet>> ParseXmlAsync(StreamReader streamReader);

        /// <summary>
        /// Parse xml by given streamReader. It has callback with Parameter as a DataSet and if it has multiple message per file, 
        /// the second parameter return per Message Unique Id and thrid parameter is the file Message Unique Id
        /// </summary>
        /// <param name="streamReader"></param>
        /// <param name="callback">Callback returns DataSet, MessageUniqueId, and UniqueID</param>
        /// <returns></returns>
        public Task ParseXmlAsync(StreamReader streamReader, Func<DataSet,Guid, Guid, Task> callback);

        /// <summary>
        /// Parse given xml file method
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>List of DataSet</returns>
        public Task<IList<DataSet>> ParseXmlAsync(string filePath);

        /// <summary>
        /// Parse given xml file method. It has callback with Parameter as a DataSet and if it has multiple message per file, 
        /// the second parameter return per Message Unique Id and thrid parameter is the file Message Unique Id
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="callback">Callback returns DataSet, MessageUniqueId, and UniqueID</param>
        /// <returns></returns>
        public Task ParseXmlAsync(string filePath, Func<DataSet, Guid, Guid, Task> callback);


    }
}
