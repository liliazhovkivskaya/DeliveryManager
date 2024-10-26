using DeliveryManager.Models.Entity;
using DeliveryManager.Service;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryManager.Controllers
{
    public class OrdersController : Controller
    {
        private readonly DeliveryContext _context;

        public OrdersController(DeliveryContext context)
        {
            _context = context;
        }

        // Метод для отображения всех заказов
        public IActionResult Index()
        {
            var orders = _context.Orders.ToList();
            return View(orders);
        }

        /// <summary>
        /// Метод для фильтрации заказов
        /// </summary>
        /// <param name="cityDistrict">Название района</param>
        /// <param name="fromDate">Дата начала</param>
        /// <param name="toDate">Дата окончания</param>
        /// <returns>Таблица с отфильтрованными данными</returns>
        public IActionResult Filter(string cityDistrict, DateTimeOffset? fromDate, DateTimeOffset? toDate)
        {
            // Проверка на корректность введенных параметров
            if (string.IsNullOrEmpty(cityDistrict) || !fromDate.HasValue || !toDate.HasValue || fromDate > toDate)
            {
                ModelState.AddModelError("", "Параметры фильтрации заданы некорректно.");
                return View(Enumerable.Empty<Order>());
            }

            // Фильтрация на стороне базы данных по строковому полю
            var filteredOrders = _context.Orders
                .Where(o => o.CityDistrict == cityDistrict)
                .AsEnumerable()
                .Where(o => o.DeliveryTime >= fromDate &&
                            o.DeliveryTime <= toDate)
                .ToList();

            // Логирование операции фильтрации
            _context.Logs.Add(new Log
            {
                Action = "Фильтрация заказов",
                Timestamp = DateTime.Now,
                Details = $"Прошла фильтрация по району \"{cityDistrict}\" с \"{fromDate}\" по \"{toDate}\". Найдено заказов: {filteredOrders.Count}"
            });
            _context.SaveChanges();

            return View(filteredOrders);
        }

        /// <summary>
        /// Создание нового заказа (метод POST)
        /// </summary>
        /// <param name="order">Объект заказа</param>
        /// <returns>Создает запись заказа в бд</returns>
        [HttpPost]
        public IActionResult Create(Order order)
        {
            order.Weight = Convert.ToDecimal(order.Weight);
            if (!ModelState.IsValid)
            {
                _context.SaveChanges();
                // Логируем ошибку валидации
                _context.Logs.Add(new Log
                {
                    Action = "Ошибка валидации при создании заказа",
                    Timestamp = DateTime.Now,
                    Details = "Не удалось создать заказ: некорректные данные."
                });

                return View(order); // Возвращаем форму с ошибками

            }
            try
            {
                order.CreatedAt = DateTime.Now;
                _context.Orders.Add(order);

                // Логирование операции добавления
                _context.Logs.Add(new Log
                {
                    Action = "Создание заказа",
                    Timestamp = DateTime.Now,
                    Details = $"Заказ с следующими характеристиками:\n Вес : {order.Weight};" +
                    $"\n Район доставки : {order.CityDistrict};" +
                    $"\n Время доставки : {order.DeliveryTime};" +
                    $"\n Время создания записи : {order.CreatedAt};"
                });
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                _context.Logs.Add(new Log
                {
                    Action = "Ошибка при создании заказа",
                    Timestamp = DateTime.Now,
                    Details = $"Ошибка: {ex.Message}"
                });
                _context.SaveChanges();

                ModelState.AddModelError("", "Произошла ошибка при создании заказа.");
                return View(order);
            }
        }

        /// <summary>
        /// Редактирование заказа (метод GET)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        /// <summary>
        /// Редактирование заказа (метод POST)
        /// </summary>
        /// <param name="order">строка заказа</param>
        /// <returns>Редактированная запись</returns>
        [HttpPost]
        public IActionResult Edit(Order order)
        {
            if (!ModelState.IsValid)
            {
                // Логирование ошибки валидации
                _context.Logs.Add(new Log
                {
                    Action = "Ошибка валидации при редактировании заказа",
                    Timestamp = DateTime.Now,
                    Details = "Не удалось редактировать заказ: некорректные данные."
                });
                _context.SaveChanges();

                return View(order);
            }
            try
            {
                order.CreatedAt = DateTime.Now;
                _context.Orders.Update(order);
                // Логирование операции добавления
                _context.Logs.Add(new Log
                {
                    Action = "Обновление заказа",
                    Timestamp = DateTime.Now,
                    Details = $"Обновленные данные заказа имеют следующие характеристики:\n Вес : {order.Weight};" +
                    $"\n Район доставки : {order.CityDistrict};" +
                    $"\n Время доставки : {order.DeliveryTime};" +
                    $"\n Время создания записи : {order.CreatedAt};"
                });
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                _context.Logs.Add(new Log
                {
                    Action = "Ошибка при редактированнии заказа",
                    Timestamp = DateTime.Now,
                    Details = $"Ошибка: {ex.Message}"
                });
                _context.SaveChanges();

                ModelState.AddModelError("", "Произошла ошибка при редактированнии заказа.");
                return View(order);
            }
        }

        /// <summary>
        /// Удаление заказа
        /// </summary>
        /// <param name="id">Id заказа</param>
        /// <returns>Удаление запис и из бд</returns>
        public IActionResult Delete(int id)
        {
            var order = _context.Orders.Find(id);
            try
            {
                _context.Orders.Remove(order);
                // Логирование операции добавления
                _context.Logs.Add(new Log
                {
                    Action = "Удаление заказа",
                    Timestamp = DateTime.Now,
                    Details = $"Заказа имеющий следующие характеристики:\n Вес : {order.Weight};" +
                    $"\n Район доставки : {order.CityDistrict};" +
                    $"\n Время доставки : {order.DeliveryTime};" +
                    $"\n Время создания записи : {order.CreatedAt}. Был удален!"
                });

                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                _context.Logs.Add(new Log
                {
                    Action = "Ошибка при удалении заказа",
                    Timestamp = DateTime.Now,
                    Details = $"Ошибка: {ex.Message}"
                });
                _context.SaveChanges();

                ModelState.AddModelError("", "Произошла ошибка при удалении заказа.");
                return View(order);
            }

        }
    }
}
