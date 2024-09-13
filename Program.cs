using System.Xml.Serialization;
using TestXML.Context;
using TestXML.Models;
using TestXML.Models.Dto;
using TestXML.Repositories;

public class Program
{
    static void Main(string[] args)
    {
        string xmlFilePath = @"data.xml";

        try
        {
            List<OrderXml> orders = ReadOrdersFromXml(xmlFilePath);
            UseOrm(orders);
            Console.WriteLine($"Данные успешно загружены в базу данных. Загружено {orders.Count} заказов.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
        }
    }

    private static List<OrderXml> ReadOrdersFromXml(string filePath)
    {
        var serializer = new XmlSerializer(typeof(RootOrders));
        using (var reader = File.OpenText(filePath))
        {
            var rootOrders = (RootOrders)serializer.Deserialize(reader);
            return rootOrders.OrdersList;
        }
    }

    private static void UseOrm(List<OrderXml> orders)
    {
        var dbContext = new MarketDbContext();

        dbContext.Database.EnsureCreated();

        try
        {
            dbContext.Database.BeginTransaction();

            var orderRepository = new OrderRepository(dbContext);
            var userRepository = new UserRepository(dbContext);
            var productRepository = new ProductRepository(dbContext);
            var productOrderRepository = new ProductOrderRepository(dbContext);


            foreach (var orderXml in orders)
            {
                var order = new Order();

                var user = userRepository.FindByEmail(orderXml.User.Email);

                if (user == null)
                {
                    user = new User()
                    {
                        Fio = orderXml.User.Fio,
                        Email = orderXml.User.Email
                    };
                }

                order.User = user;
                order.No = orderXml.No;
                order.Reg_Date = orderXml.Reg_Date;
                order.Sum = orderXml.Sum;

                foreach (var productXml in orderXml.Products)
                {
                    var product = productRepository.FindByName(productXml.Name);

                    if (product == null)
                    {
                        product = new Product { Name = productXml.Name, Price = productXml.Price };
                        productRepository.Add(product);
                    }

                    var productOrder = new ProductOrder()
                    {
                        Product = product,
                        Order = order,
                        Quantity = productXml.Quantity
                    };

                    productOrderRepository.Add(productOrder);
                }

                orderRepository.Add(order);
                dbContext.SaveChanges();
            }

            dbContext.Database.CommitTransaction();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            dbContext.Dispose();
        }
    }
}