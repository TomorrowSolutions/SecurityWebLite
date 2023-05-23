using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SecurityLite.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        [Display(Name = "Договор")]
        [Required(ErrorMessage = "Укажите договор")]
        public int OrderId { get; set; }
        [Display(Name = "Договор")]
        public Order? Order { get; set; }
        [Display(Name = "Объект")]
        [Required(ErrorMessage = "Укажите объект")]
        public int GuardedObjectId { get; set; }
        [Display(Name = "Объект")]
        public GuardedObject? GuardedObject { get; set; }
        [Display(Name = "Услуга")]
        [Required(ErrorMessage = "Укажите услугу")]
        public int ServiceId { get; set; }
        [Display(Name = "Услуга")]
        public Service? Service { get; set; }
        [Display(Name = "Кол-во услуг")]
        [Required(ErrorMessage = "Введите кол-во услуг")]
        public int Quantity { get; set; }
    }
}
