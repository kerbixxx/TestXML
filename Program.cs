using System.Xml.Serialization;
using TestXML.Models;
using TestXML.Services;

public class Program
{
    static void Main(string[] args)
    {
        string xmlFilePath = @"data.xml";

        try
        {
            List<Order> orders = ReadOrdersFromXml(xmlFilePath);
            DatabaseService.SaveDataToDatabase(orders);
            Console.WriteLine($"Данные успешно загружены в базу данных. Загружено {orders.Count} заказов.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
        }
    }

    private static List<Order> ReadOrdersFromXml(string filePath)
    {
        var serializer = new XmlSerializer(typeof(RootOrders));
        using (var reader = File.OpenText(filePath))
        {
            var rootOrders = (RootOrders)serializer.Deserialize(reader);
            return rootOrders.OrdersList;
        }
    }
}