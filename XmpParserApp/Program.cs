// See https://aka.ms/new-console-template for more information
using System.Data;
using XmlParser;
using XmlParser.Services;
using XmlParser.Source;

internal class Program
{
    private static async Task Main(string[] args)
    {
        IParsingService parsingService = new ParsingService(new ParsingJsonRules(@"C:\source\data\camt.053_rules.json"));
        await parsingService.ParseXmlAsync("C:\\source\\data\\camt.053_test_1.xml", (DataSet ds) =>
        {
            Task task = Task.Run(() =>
            {
                Console.WriteLine(ds.Tables.Count);
                ds = null;
            });
            return task;
        });
        var ds = await parsingService.ParseXmlAsync("C:\\source\\data\\camt.053_test.xml");
        //Console.WriteLine(ds.Count);
        //ds = null;
        Console.ReadLine();
    }
}