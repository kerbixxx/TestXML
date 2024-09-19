using Microsoft.EntityFrameworkCore;
using TestXML.Context;
using TestXML.Implementation.Repositories;
using TestXML.Implementation.Services;
using TestXML.Interfaces.Services;
using TestXML.Models.Dto;
public class Program
{
    static void Main(string[] args)
    {
        string xmlFilePath = @"data.xml";

        DbContextOptionsBuilder dbOptions = new DbContextOptionsBuilder()
            .UseSqlServer(@"Server=.\SQLEXPRESS;Database=testXml;Trusted_Connection=True;Integrated Security=True;TrustServerCertificate=true;");

        var dbContext = new MarketDbContext(dbOptions.Options);

        var orderRepository = new OrderRepository(dbContext);
        var userRepository = new UserRepository(dbContext);
        var productRepository = new ProductRepository(dbContext);
        var productOrderRepository = new ProductOrderRepository(dbContext);
        var transactionManager = new TransactionManager(dbContext);

        OrderProcessor orderProcessor = new(orderRepository, productOrderRepository, productRepository, userRepository, transactionManager);

        List<OrderXml> orders = orderProcessor.ReadOrdersFromXml(xmlFilePath);

        try
        {
            orderProcessor.ProcessOrders(orders);

            Console.WriteLine($"Данные успешно загружены в базу данных. Обработано {orders.Count} заказов.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
        }
    }
}