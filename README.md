# ISO 20022 Library
[![NuGet Version](https://img.shields.io/nuget/v/ISO20022.Net)](https://www.nuget.org/packages/ISO20022.Net/)
![NuGet Downloads](https://img.shields.io/nuget/dt/ISO20022.Net)

ISO20022 is the dotnet library which help to parse ISO 200022 messages in the fastes way possible with the predefined rule sets. It also helps to create ISO 20022 Messages as well as convert all ISO20022 XSD to normal JSON files.

**NOTE: MXParser has been deprecated. Please switch to ISO20022**

## Features
* Parse the ISO 20022 XML Message with pre-defined rules.
  * Rules can be in JSON structure or Database structured.
* Convert ISO20022 XSD to [ngx-iso-form](https://www.npmjs.com/package/ngx-iso-form) compaitible JSON.
  * This help to draw user interface using angular.
* Convert ngx-iso-form output JSON to ISO 20022 Message.

## Install the `ISO20022` NuGet package.
  * .NET CLI
  ```cs
    dotnet add package ISO20022.Net
  ```
  * Package Manager
  ```cs
  Install-Package ISO20022.Net
  ```

## Features in details:

### Parsing the ISO 20022 Messages


```C#
using System.Data;
using System.Diagnostics;
using ISO20022;

public class Program
{
    private static async Task Main(string[] args)
    {
        Stopwatch stopwatch = new();
        stopwatch.Start();
        IParsingService parsingService = new ParsingService(new ParsingJsonRules(@"data\parsing_rules.json"), "Document");
        await parsingService.ParseXmlAsync(@"data\camt.053.xml", (DataSet ds, Guid messageUniqueId, Guid uniquiId) =>
        {
          //DataSet ds: camt.053.xml will have multiple messages in a file and this dataset will have the parsed data of the given message. Each message parsed will be callbacked over here for the further process.
           // messageUniqueId :  As camt.053 will have multiple message, each message will identified with the given messageUniqueId. 
           // uniqueId: this id is represent the unique file Id. Meaning each passed file has been represent by the given Id. 
            Task task = Task.Run(() =>
            {
                // either we can transform to CSV/Excel or transfered to DB using bulk DB operation
                string dateTime = DateTime.Now.ToString("ddmmyyyyHHmmssfff");
                foreach(DataTable dt in ds.Tables)
                {
                    ExportDataTableToCsv(dt, @$"export\{messageUniqueId}_{dt.TableName}_{uniquiId}.csv");
                }
               
            });
            return task;
        });
        stopwatch.Stop();
        TimeSpan elapsedTime = stopwatch.Elapsed;

        // Print the elapsed time
        Console.WriteLine("Elapsed Time: " + elapsedTime);
        Console.Read();
    }

    static void ExportDataTableToCsv(DataTable dataTable, string filePath)
    {
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();
        }
        // Create a StreamWriter to write to the CSV file
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Write the header row with column names
            foreach (DataColumn column in dataTable.Columns)
            {
                writer.Write(column.ColumnName);
                writer.Write(",");
            }
            writer.WriteLine(); // Move to the next line

            // Write the data rows
            foreach (DataRow row in dataTable.Rows)
            {
                foreach (object item in row.ItemArray)
                {
                    writer.Write(item);
                    writer.Write(",");
                }
                writer.WriteLine(); // Move to the next line
            }
        }
    }

}
```
**NOTE
The library can parse the given 60.6MB xml data file in 00:00:19 seconds with 4 Cores 8 Logical Processor with 16 MB of RAM.**

### Parsing Rules
Please follow the Parsing Rules example given in the data directory of the repository.
Mapping Rules should follow below model class structure

```C#
public class MappingRules
{
    public string Namespace { get; set; }
    public List<MappingTable> Mappings { get; set; }
}

public class MappingTable
{
    public string NodeName { get; set; }
    public string XPath { get; set; }
    public IList<MappingColumn> Columns { get; set;}
}

public class MappingColumn
{
    public int OrderNumber { get; set; }
    public string NodeName { get; set; }
    public string XPath { get; set; }
    public string DataType { get; set; }
    public int MinLength { get; set; }
    public int MaxLength { get; set; }
    public int fractionDigits { get; set; }
    public int totalDigits { get; set; }
}
```
### Override the Parsing Rules
The given example is in json file. Suppose you want to store Parsing rules in Database. Now you need to override the ParsingRules class method DeserializeRules.

For Example:
```C#
public class ParsingDatabaseRules : ParsingRules
{
    public override void DeserializeRules()
    {
      //MapParsingRules is of type IList<MappingRules>?
        MapParsingRules = GetMyRulesFromDB();
    }
}
```

### Sample Files

[camt.053.xml](https://github.com/pixelbyaj/data/raw/main/MParser/camt.053.xml)

[parsing_rules.xml](https://github.com/pixelbyaj/data/raw/main/MParser/parsing_rules.json)

## Convert ISO 20022 XSD to JSON

```C#
using ISO20022;

var fileInfo = new FileInfo(fileName);
if (File.Exists(fileName) && fileInfo.Extension.Equals(".xsd"))
{
    XsdToJson xsdLib = new(fileName);
    xsdLib.Convert();
    File.AppendAllText(fileInfo.FullName.Replace(".xsd", ".json"), xsdLib.SchemaJson);
}
```

### Model of JSON
```c#
public class XsdSchema
{
    public required string Namespace { get; set; }
    public required SchemaElement SchemaElement { get; set; }
}

public class SchemaElement
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? DataType { get; set; }
    public string? MinOccurs { get; set; }
    public string? MaxOccurs { get; set; }
    public string? MinLength { get; set; }
    public string? MaxLength { get; set; }
    public string? Pattern { get; set; }
    public string? FractionDigits { get; set; }
    public string? TotalDigits { get; set; }
    public string? MinInclusive { get; set; }
    public string? MaxInclusive { get; set; }
    public string[]? Values { get; set; }
    public bool IsCurrency { get; set; }
    public string? XPath { get; set; }
    public List<SchemaElement> Elements { get; set; }
}
```
### Example
```json
{
    "namespace": "urn:iso:std:iso:20022:tech:xsd:camt.053.001.10",
    "schemaElement": {
        "id": "Document",
        "name": "Document",
        "dataType": null,
        "minOccurs": "1",
        "maxOccurs": null,
        "minLength": null,
        "maxLength": null,
        "pattern": null,
        "fractionDigits": null,
        "totalDigits": null,
        "minInclusive": null,
        "maxInclusive": null,
        "values": null,
        "isCurrency": false,
        "xpath": "Document",
        "elements":[
          ...
        ]
    }
}
```
### Notes
  * Throw JsonException in the case where it found deep nested node structure.
  * CurrentDepth (64) is equal to or larger than the maximum allowed depth of (64)

## Convert JSON to ISO 20022 Message 

**Note: JSON here is the output of ngx-iso-form angular component**

### Example

```c#

using ISO20022;
string targetNamespace = "urn:iso:std:iso:20022:tech:xsd:camt.053.001.10";
string jsonData = File.ReadAllText(@jsonPath);
string xsdContent = File.ReadAllText(@xsdFilePath);
XElement xml = MxMessage.Create(jsonData, targetNamespace) ?? throw new Exception("Conversion failed");
if (MxMessage.ValidateMXMessage(xsdContent, xml.ToString(), out string validationMessage))
{
    if (string.IsNullOrEmpty(validationMessage))
    {
        Console.WriteLine(xml?.ToString());
    }
    else
    {
       Console.Error.WriteLine(validationMessage);
    }
}
```
JSON Example

  ```json
  {
    "Document": {
        "Document_BkToCstmrStmt": {
            "Document_BkToCstmrStmt_GrpHdr": {
                "Document_BkToCstmrStmt_GrpHdr_MsgId": "235549650",
                "Document_BkToCstmrStmt_GrpHdr_CreDtTm": "2023-10-05T14:43:51.979",
                "Document_BkToCstmrStmt_GrpHdr_MsgRcpt": {
                    "Document_BkToCstmrStmt_GrpHdr_MsgRcpt_Nm": "Test Client Ltd.",
                    "Document_BkToCstmrStmt_GrpHdr_MsgRcpt_Id": {
                        "Document_BkToCstmrStmt_GrpHdr_MsgRcpt_Id_OrgId": {
                            "Document_BkToCstmrStmt_GrpHdr_MsgRcpt_Id_OrgId_Othr": [
                                {
                                    "Document_BkToCstmrStmt_GrpHdr_MsgRcpt_Id_OrgId_Othr_Id": "test001"
                                }
                            ]
                        }
                    }
                },
                "Document_BkToCstmrStmt_GrpHdr_AddtlInf": "AddTInf"
            },
            "Document_BkToCstmrStmt_Stmt": [
                {
                    "Document_BkToCstmrStmt_Stmt_Id": "258158850",
                    "Document_BkToCstmrStmt_Stmt_ElctrncSeqNb": "1",
                    "Document_BkToCstmrStmt_Stmt_LglSeqNb": "1",
                    "Document_BkToCstmrStmt_Stmt_CreDtTm": "2023-10-05T14:43:52.098",
                    "Document_BkToCstmrStmt_Stmt_FrToDt": {
                        "Document_BkToCstmrStmt_Stmt_FrToDt_FrDtTm": "2023-09-30T20:00:00.000",
                        "Document_BkToCstmrStmt_Stmt_FrToDt_ToDtTm": "2023-10-01T19:59:59.000"
                    },
                    "Document_BkToCstmrStmt_Stmt_Acct": {
                        "Document_BkToCstmrStmt_Stmt_Acct_Tp": {
                            "Document_BkToCstmrStmt_Stmt_Acct_Tp_Prtry": "IBDA_DDA"
                        },
                        "Document_BkToCstmrStmt_Stmt_Acct_Ccy": "USD",
                        "Document_BkToCstmrStmt_Stmt_Acct_Nm": "Sample Name 123",
                        "Document_BkToCstmrStmt_Stmt_Acct_Svcr": {
                            "Document_BkToCstmrStmt_Stmt_Acct_Svcr_FinInstnId": {
                                "Document_BkToCstmrStmt_Stmt_Acct_Svcr_FinInstnId_BICFI": "GSCRUS30",
                                "Document_BkToCstmrStmt_Stmt_Acct_Svcr_FinInstnId_Nm": "Goldman Sachs Bank"
                            }
                        }
                    },
                    "Document_BkToCstmrStmt_Stmt_Bal": [
                        {
                            "Document_BkToCstmrStmt_Stmt_Bal_Tp": {
                                "Document_BkToCstmrStmt_Stmt_Bal_Tp_CdOrPrtry": {
                                    "Document_BkToCstmrStmt_Stmt_Bal_Tp_CdOrPrtry_Cd": "OPBD"
                                }
                            },
                            "Document_BkToCstmrStmt_Stmt_Bal_Amt": {
                                "Document_BkToCstmrStmt_Stmt_Bal_Amt_Ccy": "USD",
                                "Document_BkToCstmrStmt_Stmt_Bal_Amt_Amt": "843686.20"
                            },
                            "Document_BkToCstmrStmt_Stmt_Bal_CdtDbtInd": "DBIT",
                            "Document_BkToCstmrStmt_Stmt_Bal_Dt": {
                                "Document_BkToCstmrStmt_Stmt_Bal_Dt_DtTm": "2023-09-30T20:00:00.000"
                            }
                        },
                        {
                            "Document_BkToCstmrStmt_Stmt_Bal_Tp": {
                                "Document_BkToCstmrStmt_Stmt_Bal_Tp_CdOrPrtry": {
                                    "Document_BkToCstmrStmt_Stmt_Bal_Tp_CdOrPrtry_Cd": "CLAV"
                                }
                            },
                            "Document_BkToCstmrStmt_Stmt_Bal_Amt": {
                                "Document_BkToCstmrStmt_Stmt_Bal_Amt_Ccy": "USD",
                                "Document_BkToCstmrStmt_Stmt_Bal_Amt_Amt": "334432401.27"
                            },
                            "Document_BkToCstmrStmt_Stmt_Bal_CdtDbtInd": "CRDT",
                            "Document_BkToCstmrStmt_Stmt_Bal_Dt": {
                                "Document_BkToCstmrStmt_Stmt_Bal_Dt_DtTm": "2023-10-01T23:59:00.000Z"
                            }
                        }
                    ]
                }
            ]
        }
    }
}
  ```
After Conversion
  ```xml
  <Document xmlns="urn:iso:std:iso:20022:tech:xsd:camt.053.001.10">
  <BkToCstmrStmt>
    <GrpHdr>
      <MsgId>235549650</MsgId>
      <CreDtTm>2023-10-05T14:43:51.979</CreDtTm>
      <MsgRcpt>
        <Nm>Test Client Ltd.</Nm>
        <Id>
          <OrgId>
            <Othr>
              <Id>test001</Id>
            </Othr>
          </OrgId>
        </Id>
      </MsgRcpt>
      <AddtlInf>AddTInf</AddtlInf>
    </GrpHdr>
    <Stmt>
      <Id>258158850</Id>
      <ElctrncSeqNb>1</ElctrncSeqNb>
      <LglSeqNb>1</LglSeqNb>
      <CreDtTm>2023-10-05T14:43:52.098</CreDtTm>
      <FrToDt>
        <FrDtTm>2023-09-30T20:00:00.000</FrDtTm>
        <ToDtTm>2023-10-01T19:59:59.000</ToDtTm>
      </FrToDt>
      <Acct>
        <Tp>
          <Prtry>IBDA_DDA</Prtry>
        </Tp>
        <Ccy>USD</Ccy>
        <Nm>Sample Name 123</Nm>
        <Svcr>
          <FinInstnId>
            <BICFI>GSCRUS30</BICFI>
            <Nm>Goldman Sachs Bank</Nm>
          </FinInstnId>
        </Svcr>
      </Acct>
      <Bal>
        <Tp>
          <CdOrPrtry>
            <Cd>OPBD</Cd>
          </CdOrPrtry>
        </Tp>
        <Amt Ccy="USD">843686.20</Amt>
        <CdtDbtInd>DBIT</CdtDbtInd>
        <Dt>
          <DtTm>2023-09-30T20:00:00.000</DtTm>
        </Dt>
      </Bal>
      <Bal>
        <Tp>
          <CdOrPrtry>
            <Cd>CLAV</Cd>
          </CdOrPrtry>
        </Tp>
        <Amt Ccy="USD">334432401.27</Amt>
        <CdtDbtInd>CRDT</CdtDbtInd>
        <Dt>
          <DtTm>2023-10-01T23:59:00.000Z</DtTm>
        </Dt>
      </Bal>
    </Stmt>
  </BkToCstmrStmt>
</Document>
  ```
