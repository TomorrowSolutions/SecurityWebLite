using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SecurityLite.Models
{
    public class Order
    {
        public int Id { get; set; }
        [Display(Name = "Менеджер")]
        [Required(ErrorMessage = "Укажите менеджера")]
        public int ManagerId { get; set; }
        [Display(Name = "Менеджер")]
        public Manager? Manager { get; set; }
        [Display(Name = "Клиент")]
        [Required(ErrorMessage = "Укажите клиента")]
        public int ClientId { get; set; }
        [Display(Name = "Клиент")]
        public Client? Client { get; set; }
        [Display(Name = "Дата подписания")]
        [Required(ErrorMessage = "Укажите дату подписания")]
        public DateTime DateOfSigning { get; set; }
        [Display(Name = "Дата выполнения")]
        public DateTime? DateOfComplete { get; set; }
        [Display(Name = "Цена")]
        public double? price { get; set; }
    }
}
