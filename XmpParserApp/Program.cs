// See https://aka.ms/new-console-template for more information
using XmlParser;
using XmlParser.Services;
using XmlParser.Source;

internal class Program
{
    private static async Task Main(string[] args)
    {
        IParsingService parsingService = new ParsingService(new ParsingJsonRules(@"C:\source\data\camt.053_rules.json"));
        var data = await parsingService.ParseXmlAsync("C:\\source\\data\\camt.053_test.xml");
        Console.WriteLine(data.Count.ToString());
        Console.ReadLine();
    }
}