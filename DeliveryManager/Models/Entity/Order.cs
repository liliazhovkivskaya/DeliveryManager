using System.ComponentModel.DataAnnotations;

namespace DeliveryManager.Models.Entity
{
    public class Order
    {
        public int OrderId { get; set; }  // Уникальный идентификатор заказа

        [Range(0.1, 1000, ErrorMessage = "Вес должен быть в диапазоне от 0.1 до 1000 кг.")]
        public decimal Weight { get; set; }  // Вес заказа в килограммах

        [Required(ErrorMessage = "Район доставки обязателен.")]
        [StringLength(100, ErrorMessage = "Название района не должно превышать 100 символов.")]
        public string CityDistrict { get; set; }  // Район доставки

        [Required(ErrorMessage = "Время доставки обязательно.")]
        public DateTimeOffset DeliveryTime { get; set; }  // Время доставки заказа

        public DateTimeOffset CreatedAt { get; set; }  // Время создания записи
    }
}
