// See https://aka.ms/new-console-template for more information
using System.Data;
using System.Diagnostics;
using MXParser.Services;
using MXParser.Source;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        IParsingService parsingService = new ParsingService(new ParsingJsonRules(@"C:\source\data\camt.053_rules.json"), "Document");
        await parsingService.ParseXmlAsync("C:\\source\\data\\camt.053_test.xml", (DataSet ds, Guid messageUniqueId, Guid uniquiId) =>
        {
            Task task = Task.Run(() =>
            {
                string dateTime = DateTime.Now.ToString("ddmmyyyyHHmmssfff");
                foreach(DataTable dt in ds.Tables)
                {
                    ExportDataTableToCsv(dt, @$"C:\source\data\export\{messageUniqueId}_{dt.TableName}_{uniquiId}.csv");
                }
               
            });
            return task;
        });
        stopwatch.Stop();
        TimeSpan elapsedTime = stopwatch.Elapsed;

        // Print the elapsed time
        Console.WriteLine("Elapsed Time: " + elapsedTime);
        Console.Read();
        //var ds = await parsingService.ParseXmlAsync("C:\\source\\data\\camt.053_test.xml");
        //Console.WriteLine(ds.Count);
        //ds = null;
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