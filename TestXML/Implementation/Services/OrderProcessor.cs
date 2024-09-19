using System.Xml.Serialization;
using TestXML.Context;
using TestXML.Implementation.Repositories;
using TestXML.Interfaces.Services;
using TestXML.Models;
using TestXML.Models.Dto;

namespace TestXML.Implementation.Services
{
    public class OrderProcessor : IOrderProcessor
    {
        private readonly OrderRepository _orderRepo;
        private readonly ProductOrderRepository _productOrderRepo;
        private readonly ProductRepository _productRepo;
        private readonly UserRepository _userRepo;

        public OrderProcessor(OrderRepository orderRepository,
                               ProductOrderRepository productOrderRepository,
                               ProductRepository productRepository,
                               UserRepository userRepository)
        {
            _orderRepo = orderRepository;
            _productRepo = productRepository;
            _userRepo = userRepository;
            _productOrderRepo = productOrderRepository;
        }

        public List<OrderXml> ReadOrdersFromXml(string filePath)
        {
            var serializer = new XmlSerializer(typeof(RootOrders));
            using (var reader = File.OpenText(filePath))
            {
                var rootOrders = (RootOrders)serializer.Deserialize(reader);
                return rootOrders.OrdersList;
            }
        }

        public void ProcessOrders(List<OrderXml> orders)
        {
            var dbContext = new MarketDbContext();

            dbContext.Database.EnsureCreated();

            var orderRepository = new OrderRepository(dbContext);
            var userRepository = new UserRepository(dbContext);
            var productRepository = new ProductRepository(dbContext);
            var productOrderRepository = new ProductOrderRepository(dbContext);

            try
            {
                dbContext.Database.BeginTransaction();

                foreach (var orderXml in orders)
                {
                    ProcessOrder(orderXml);
                }

                dbContext.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                dbContext.Database.RollbackTransaction();
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        private void ProcessOrder(OrderXml orderXml)
        {
            var existingOrder = _orderRepo.FindByNo(orderXml.No);

            if (existingOrder != null)
            {
                UpdateExistingOrder(existingOrder, orderXml);
            }
            else
            {
                var order = CreateOrder(orderXml);
                AddProductsToOrder(order, orderXml.Products);
                _orderRepo.Add(order);
            }

            _orderRepo.Save();
        }

        private Order CreateOrder(OrderXml orderXml)
        {
            var user = _userRepo.FindByEmail(orderXml.User.Email);
            if (user == null)
            {
                user = CreateUser(orderXml.User);
                _userRepo.Add(user);
            }

            var order = new Order();
            order.User = user;
            order.No = orderXml.No;
            order.Reg_Date = orderXml.Reg_Date;
            order.Sum = orderXml.Sum;
            return order;
        }

        private User CreateUser(UserXml userXml)
        {
            return new User { Fio = userXml.Fio, Email = userXml.Email };
        }

        private void AddProductsToOrder(Order order, List<ProductXml> products)
        {
            foreach (var productXml in products)
            {
                var product = _productRepo.FindByName(productXml.Name);
                if (product == null)
                {
                    product = new Product { Name = productXml.Name, Price = productXml.Price };
                    _productRepo.Add(product);
                }

                var productOrder = new ProductOrder { Product = product, Order = order, Quantity = productXml.Quantity };
                _productOrderRepo.Add(productOrder);
            }
        }

        private void UpdateExistingOrder(Order existingOrder, OrderXml orderXml)
        {
            existingOrder.User = _userRepo.FindByEmail(orderXml.User.Email);
            existingOrder.No = orderXml.No;
            existingOrder.Reg_Date = orderXml.Reg_Date;
            existingOrder.Sum = orderXml.Sum;

            foreach (var productXml in orderXml.Products)
            {
                var productId = _productRepo.FindByName(productXml.Name).Id;
                var existingProductOrder = _productOrderRepo.FindByProductIdAndOrderId(existingOrder.Id, productId);

                if (existingProductOrder == null)
                {
                    var product = _productRepo.FindByName(productXml.Name);
                    if (product == null)
                    {
                        product = new Product { Name = productXml.Name, Price = productXml.Price };
                        _productRepo.Add(product);
                    }

                    existingProductOrder = new ProductOrder { Product = product, Order = existingOrder, Quantity = productXml.Quantity };
                    _productOrderRepo.Add(existingProductOrder);
                }
                else
                {
                    existingProductOrder.Quantity = productXml.Quantity;
                }
            }
        }
    }
}
