using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SecurityLite.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Display(Name = "Название")]
        [Required(ErrorMessage = "Введите название")]
        public string Name { get; set; }
        [Display(Name = "Оклад")]
        [Required(ErrorMessage = "Введите оклад")]
        public double Salary { get; set; }
    }
}
