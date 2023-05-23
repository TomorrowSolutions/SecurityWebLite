using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SecurityLite.Models
{
    public class Service
    {
        public int Id { get; set; }
        [Display(Name = "Название")]
        [StringLength(100, MinimumLength = 1)]
        [Required(ErrorMessage = "Введите название")]
        public string Name { get; set; }
        [Display(Name = "Стоимость")]
        [Required(ErrorMessage = "Введите стоимость")]
        public double Price { get; set; }
        [Display(Name = "Время выполнения(дни)")]
        [Required(ErrorMessage = "Введите время выполнения в днях")]
        public int PeriodOfExecution { get; set; }
        [Display(Name = "Описание услуги")]
        public string? Description { get; set; }
    }
}
