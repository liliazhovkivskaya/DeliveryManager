namespace DeliveryManager.Models.Entity
{
    public class Log
    {
        public int LogId { get; set; }  // Уникальный идентификатор записи в логе
        public string? Action { get; set; }  // Описание действия
        public DateTime Timestamp { get; set; }  // Время записи
        public string? Details { get; set; }  // Дополнительные сведения о действии
    }
}
