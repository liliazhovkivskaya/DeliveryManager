using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeliveryManager.Service;
using DeliveryManager.Controllers;
using DeliveryManager.Models.Entity;

namespace CityDeliveryManager.Tests
{
    // Отключаем параллельное выполнение тестов для этого класса
    [Collection("Sequential")]
    public class OrdersControllerTests : IDisposable
    {
        private readonly DeliveryContext _context;
        private readonly OrdersController _controller;

        public OrdersControllerTests()
        {
            // Генерируем уникальное имя базы данных для каждого теста
            var options = new DbContextOptionsBuilder<DeliveryContext>()
                            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                            .Options;

            // Инициализируем контекст и контроллер
            _context = new DeliveryContext(options);
            _controller = new OrdersController(_context);

            // Заполняем контекст тестовыми данными
            _context.Orders.AddRange(new List<Order>
            {
                new Order { OrderId = 1, CityDistrict = "Гидростоителей", DeliveryTime = DateTimeOffset.Now },
                new Order { OrderId = 2, CityDistrict = "Карасунский", DeliveryTime = DateTimeOffset.Now.AddHours(1) }
            });
            _context.SaveChanges();
        }

        [Fact]
        public void Index_ReturnsViewWithOrders()
        {
            // Act
            var result = _controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<Order>>(result.Model);
            var model = result.Model as List<Order>;
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public void Filter_ReturnsEmptyViewWhenParametersInvalid()
        {
            // Act
            var result = _controller.Filter("", null, null) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<Order>>(result.Model);
            Assert.True(result.ViewData.ModelState.ErrorCount > 0);
        }

        [Fact]
        public void Create_ReturnsViewWithOrderWhenModelStateInvalid()
        {
            // Arrange
            var order = new Order { OrderId = 1, CityDistrict = "District1", Weight = -1 };
            _controller.ModelState.AddModelError("Weight", "Invalid weight");

            // Act
            var result = _controller.Create(order) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order, result.Model);
        }

        [Fact]
        public void Edit_ReturnsNotFoundWhenOrderNotExists()
        {
            // Act
            var result = _controller.Edit(999) as NotFoundResult;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Delete_RemovesOrderAndReturnsToIndex()
        {
            // Arrange
            // Проверяем, что заказ с ID 1 существует
            var order = _context.Orders.Find(1);
            Assert.NotNull(order);

            // Act
            var result = _controller.Delete(1) as RedirectToActionResult;

            // Assert
            Assert.Equal("Index", result.ActionName);

            // Проверяем, что заказ был удален
            var deletedOrder = _context.Orders.Find(1);
            Assert.Null(deletedOrder);
        }

        public void Dispose()
        {
            // Очищаем базу данных после каждого теста
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
