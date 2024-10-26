using DeliveryManager.Service;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryManager.Controllers
{
    public class LoggingController : Controller
    {
        private readonly DeliveryContext _context;

        public LoggingController(DeliveryContext context)
        {
            _context = context;
        }

        // Отображение всех логов
        public IActionResult Index()
        {
            var logs = _context.Logs.OrderByDescending(log => log.Timestamp).ToList();
            return View(logs);
        }
    }
}
