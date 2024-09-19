using TestXML.Context;
using TestXML.Implementation.Repositories;
using TestXML.Implementation.Services;
using TestXML.Models.Dto;
public class Program
{
    static void Main(string[] args)
    {
        string xmlFilePath = @"data.xml";

        var dbContext = new MarketDbContext();

        var orderRepository = new OrderRepository(dbContext);
        var userRepository = new UserRepository(dbContext);
        var productRepository = new ProductRepository(dbContext);
        var productOrderRepository = new ProductOrderRepository(dbContext);

        OrderProcessor orderProcessor = new(orderRepository, productOrderRepository, productRepository, userRepository);

        List<OrderXml> orders = orderProcessor.ReadOrdersFromXml(xmlFilePath);

        try
        {
            orderProcessor.ProcessOrders(orders);

            Console.WriteLine($"Данные успешно загружены в базу данных. Загружено {orders.Count} заказов.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
        }
    }
}