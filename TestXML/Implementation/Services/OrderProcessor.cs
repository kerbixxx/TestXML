using System.Xml.Serialization;
using TestXML.Context;
using TestXML.Implementation.Repositories;
using TestXML.Interfaces.Repositories;
using TestXML.Interfaces.Services;
using TestXML.Models;
using TestXML.Models.Dto;

namespace TestXML.Implementation.Services
{
    public class OrderProcessor : IOrderProcessor
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IProductOrderRepository _productOrderRepo;
        private readonly IProductRepository _productRepo;
        private readonly IUserRepository _userRepo;
        private readonly ITransactionManager _transactionManager;

        public OrderProcessor(IOrderRepository orderRepository,
                               IProductOrderRepository productOrderRepository,
                               IProductRepository productRepository,
                               IUserRepository userRepository,
                               ITransactionManager transactionManager)
        {
            _orderRepo = orderRepository;
            _productRepo = productRepository;
            _userRepo = userRepository;
            _productOrderRepo = productOrderRepository;
            _transactionManager = transactionManager;
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
            try
            {
                _transactionManager.BeginTransaction();

                foreach (var orderXml in orders)
                {
                    ProcessOrder(orderXml);
                }

                _transactionManager.CommitTransaction();
            }
            catch (Exception ex)
            {
                _transactionManager.RollbackTransaction();
                throw ex;
            }
            finally
            {
                _transactionManager.Dispose();
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
            // Обновляем общую информацию о заказе
            existingOrder.User = _userRepo.FindByEmail(orderXml.User.Email);
            existingOrder.No = orderXml.No;
            existingOrder.Reg_Date = orderXml.Reg_Date;
            existingOrder.Sum = orderXml.Sum;

            _productOrderRepo.RemoveRange(existingOrder.Id);

            foreach (var productXml in orderXml.Products)
            {
                var product = _productRepo.FindByName(productXml.Name);
                if (product == null)
                {
                    product = new Product { Name = productXml.Name, Price = productXml.Price };
                    _productRepo.Add(product);
                }

                var quantity = productXml.Quantity;
                var existingProductOrder = _productOrderRepo.FindByProductIdAndOrderId(existingOrder.Id, product.Id);

                if (existingProductOrder != null)
                {
                    existingProductOrder.Quantity = quantity;
                }
                else
                {
                    existingProductOrder = new ProductOrder
                    {
                        Product = product,
                        Order = existingOrder,
                        Quantity = quantity
                    };
                    _productOrderRepo.Add(existingProductOrder);
                }
            }

            foreach (var existingProductOrder in _productOrderRepo.GetAllByOrderId(existingOrder.Id))
            {
                var productXml = orderXml.Products.FirstOrDefault(p => p.Name == existingProductOrder.Product.Name);
                if (productXml != null)
                {
                    existingProductOrder.Quantity = productXml.Quantity;
                }
            }

            _orderRepo.Update(existingOrder);
        }
    }
}
