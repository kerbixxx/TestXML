using Moq;
using TestXML.Context;
using TestXML.Implementation.Repositories;
using TestXML.Implementation.Services;
using TestXML.Interfaces.Repositories;
using TestXML.Models.Dto;
using TestXML.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using TestXML.Exceptions;

namespace TestXML.Tests
{
    public class OrderProcessorTests
    {
        private readonly DbContextOptions<MarketDbContext> _options;
        private MarketDbContext _context;

        private IOrderRepository _orderRepo;
        private IUserRepository _userRepository;
        private IProductRepository _productRepo;
        private IProductOrderRepository _productOrderRepo;
        private OrderProcessor _processor;
        private Mock<ITransactionManager> _transactionManager;

        public OrderProcessorTests()
        {
            var uniqueDbName = GenerateUniqueDatabaseName();
            _options = new DbContextOptionsBuilder<MarketDbContext>().UseInMemoryDatabase(uniqueDbName).Options;

            ClearDatabase(uniqueDbName);

            _context = new MarketDbContext(_options);

            _orderRepo = new OrderRepository(_context);
            _userRepository = new UserRepository(_context);
            _productRepo = new ProductRepository(_context);
            _productOrderRepo = new ProductOrderRepository(_context);
            _transactionManager = new Mock<ITransactionManager>();
            _processor = new OrderProcessor(_orderRepo, _productOrderRepo, _productRepo, _userRepository, _transactionManager.Object);
        }


        //Добавление заказов, если они не существуют.
        [Fact]
        public void ProcessOrders_ShouldCreateNewOrder_WhenOrderDoesNotExist()
        {
            ClearDatabase(GenerateUniqueDatabaseName());

            var orderXml = new OrderXml
            {
                No = 123,
                Reg_Date = DateTime.Now.ToShortDateString(),
                Sum = 20500,
                Products = new List<ProductXml>
                {
                    new ProductXml { Name = "Product1", Price = 10000, Quantity = 2 },
                    new ProductXml { Name = "Product2", Price = 500, Quantity = 1 }
                },
                User = new UserXml { Email = "test@example.com", Fio = "Test Test" }
            };

            _processor.ProcessOrders(new List<OrderXml> { orderXml });

            Assert.Single(_orderRepo.GetAll());

            var newOrder = _orderRepo.FindByNo(123);
            Assert.NotNull(newOrder);
            Assert.Equal(123, newOrder.No);
            Assert.Equal("Test Test", newOrder.User.Fio);
            Assert.Equal(20500, newOrder.Sum);

            Assert.Equal(2, _productOrderRepo.GetAll().Count());
            Assert.Equal("Product1", _productOrderRepo.FindByProductIdAndOrderId(newOrder.Id, 1).Product.Name);
            Assert.Equal(2, _productOrderRepo.FindByProductIdAndOrderId(newOrder.Id, 1).Quantity);
            Assert.Equal("Product2", _productOrderRepo.FindByProductIdAndOrderId(newOrder.Id, 2).Product.Name);
            Assert.Equal(1, _productOrderRepo.FindByProductIdAndOrderId(newOrder.Id, 2).Quantity);
        }

        //Обновление существующего заказа.
        [Fact]
        public void ProcessOrders_ShouldUpdateExistingOrder_WhenOrderExists()
        {
            ClearDatabase(GenerateUniqueDatabaseName());

            var initialOrderXml = new OrderXml
            {
                No = 123,
                Reg_Date = DateTime.Now.ToShortDateString(),
                Sum = 20500,
                Products = new List<ProductXml>
                {
                    new ProductXml { Name = "Product1", Price = 10000, Quantity = 2 },
                    new ProductXml { Name = "Product2", Price = 500, Quantity = 1 }
                },
                User = new UserXml { Email = "test@example.com", Fio = "Updated Test" }
            };

            _processor.ProcessOrders(new List<OrderXml> { initialOrderXml });

            Assert.Equal(2, _productOrderRepo.GetAll().Count());
            var initialOrder = _orderRepo.FindByNo(123);
            Assert.Equal(20500, initialOrder.Sum);

            var modifiedOrderXml = new OrderXml
            {
                No = 123,
                Reg_Date = DateTime.Now.ToShortDateString(),
                Sum = 30800,
                Products = new List<ProductXml>
                {
                    new ProductXml { Name = "Product1", Price = 10000, Quantity = 3 },
                    new ProductXml { Name = "Product2", Price = 500, Quantity = 1 },
                    new ProductXml { Name = "Product3", Price = 300, Quantity = 1 }
                },
                User = new UserXml { Email = "test@example.com", Fio = "Updated Test" }
            };

            _processor.ProcessOrders(new List<OrderXml> { modifiedOrderXml });

            var updatedOrder = _orderRepo.FindByNo(123);
            Assert.Equal(3, _productOrderRepo.GetAll().Count());
            Assert.NotNull(updatedOrder);
            Assert.Equal(123, updatedOrder.No);
            Assert.Equal("Updated Test", updatedOrder.User.Fio);
            Assert.Equal(30800, updatedOrder.Sum);
        }

        //Обработка ошибки при отсутствии пользователя
        [Fact]
        public void ProcessOrders_ShouldThrowException_WhenUserNotFoundAndCannotCreate()
        {
            ClearDatabase(GenerateUniqueDatabaseName());

            var orderXml = new OrderXml
            {
                No = 123,
                Reg_Date = DateTime.Now.ToShortDateString(),
                Sum = 20500,
                Products = new List<ProductXml>
                {
                    new ProductXml { Name = "Product1", Price = 10000, Quantity = 2 },
                    new ProductXml { Name = "Product2", Price = 500, Quantity = 1 }
                }
            };

            Assert.Throws<UserNotFoundException>(() => _processor.ProcessOrders(new List<OrderXml> { orderXml }));
        }

        //Изменение заказа на тот же.
        [Fact]
        public void ProcessOrders_ShouldNotChangeOrderWhenProductsUnchanged()
        {
            ClearDatabase(GenerateUniqueDatabaseName());

            var initialOrderXml = new OrderXml
            {
                No = 123,
                Reg_Date = DateTime.Now.ToShortDateString(),
                Sum = 20500,
                Products = new List<ProductXml>
                {
                    new ProductXml { Name = "Product1", Price = 10000, Quantity = 2 },
                    new ProductXml { Name = "Product2", Price = 500, Quantity = 1 }
                },
                User = new UserXml { Email = "test@example.com", Fio = "Updated Test" }
            };

            _processor.ProcessOrders(new List<OrderXml> { initialOrderXml });

            var initialOrder = _orderRepo.FindByNo(123);
            Assert.Equal(20500, initialOrder.Sum);
            Assert.Equal(2, _productOrderRepo.GetAll().Count());


            var orderXml = new OrderXml
            {
                No = 123,
                Reg_Date = DateTime.Now.ToShortDateString(),
                Sum = 20500,
                Products = new List<ProductXml>
                {
                    new ProductXml { Name = "Product1", Price = 10000, Quantity = 2 },
                    new ProductXml { Name = "Product2", Price = 500, Quantity = 1 }
                },
                User = new UserXml { Email = "test@example.com", Fio = "Updated Test" }
            };

            _processor.ProcessOrders(new List<OrderXml> { orderXml });

            var updatedOrder = _orderRepo.FindByNo(123);
            Assert.Equal(20500, updatedOrder.Sum);
            Assert.Equal(2, _productOrderRepo.GetAll().Count());
        }

        //Добавление нового продукта к существующему заказу.
        [Fact]
        public void ProcessOrders_ShouldAddNewProductToExistingOrder()
        {
            ClearDatabase(GenerateUniqueDatabaseName());

            var initialOrderXml = new OrderXml
            {
                No = 123,
                Reg_Date = DateTime.Now.ToShortDateString(),
                Sum = 2000,
                Products = new List<ProductXml>
                {
                    new ProductXml { Name = "Product1", Price = 1000, Quantity = 1 },
                    new ProductXml { Name = "Product2", Price = 500, Quantity = 1 }
                },
                User = new UserXml { Email = "test@example.com", Fio = "Updated Test" }
            };

            _processor.ProcessOrders(new List<OrderXml> { initialOrderXml });

            var orderXml = new OrderXml
            {
                No = 123,
                Reg_Date = DateTime.Now.ToShortDateString(),
                Sum = 3000,
                Products = new List<ProductXml>
                {
                    new ProductXml { Name = "Product1", Price = 1000, Quantity = 2 },
                    new ProductXml { Name = "Product2", Price = 500, Quantity = 1 },
                    new ProductXml { Name = "Product3", Price = 1500, Quantity = 1 }
                },
                User = new UserXml { Email = "test@example.com", Fio = "Updated Test" }
            };

            _processor.ProcessOrders(new List<OrderXml> { orderXml });

            var updatedOrder = _orderRepo.FindByNo(123);
            Assert.Equal(3000, updatedOrder.Sum);

            var productOrders = _productOrderRepo.GetAll().ToList();
            Assert.Contains("Product1", productOrders.Select(po => po.Product.Name));
            Assert.Contains("Product2", productOrders.Select(po => po.Product.Name));
            Assert.Contains("Product3", productOrders.Select(po => po.Product.Name));
        }

        //Удаление продукта из существующего заказа.
        [Fact]
        public void ProcessOrders_ShouldRemoveProductFromExistingOrder()
        {
            ClearDatabase(GenerateUniqueDatabaseName());

            var initialOrderXml = new OrderXml
            {
                No = 123,
                Reg_Date = DateTime.Now.ToShortDateString(),
                Sum = 2000,
                Products = new List<ProductXml>
                {
                    new ProductXml { Name = "Product1", Price = 1000, Quantity = 1 },
                    new ProductXml { Name = "Product2", Price = 500, Quantity = 1 }
                },
                User = new UserXml { Email = "test@example.com", Fio = "Updated Test" }
            };

            _processor.ProcessOrders(new List<OrderXml> { initialOrderXml });

            var orderXml = new OrderXml
            {
                No = 123,
                Reg_Date = DateTime.Now.ToShortDateString(),
                Sum = 1000,
                Products = new List<ProductXml>
                {
                    new ProductXml { Name = "Product2", Price = 500, Quantity = 1 }
                },
                User = new UserXml { Email = "test@example.com", Fio = "Updated Test" }
            };

            _processor.ProcessOrders(new List<OrderXml> { orderXml });

            var updatedOrder = _orderRepo.FindByNo(123);
            Assert.Equal(1000, updatedOrder.Sum);

            var productOrders = _productOrderRepo.GetAll();
            Assert.Single(productOrders);
            Assert.Equal("Product2", productOrders.First().Product.Name);
        }

        //Создание заказа без продуктов.
        [Fact]
        public void ProcessOrders_ShouldThrowException_WhenProductNotFoundAndCannotCreate()
        {
            ClearDatabase(GenerateUniqueDatabaseName());

            var orderXml = new OrderXml
            {
                No = 123,
                Reg_Date = DateTime.Now.ToShortDateString(),
                Sum = 20500,
                User = new UserXml { Email = "test@example.com", Fio = "Test Test" }
            };

            Assert.Throws<OrderProcessingException>(() => _processor.ProcessOrders(new List<OrderXml> { orderXml }));
        }


        private string GenerateUniqueDatabaseName()
        {
            return $"TestDB_{Guid.NewGuid():N}";
        }

        internal void ClearDatabase(string dbName)
        {
            using (var context = new MarketDbContext(new DbContextOptionsBuilder<MarketDbContext>().UseInMemoryDatabase(dbName).Options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
        }
    }
}
