using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SecurityLite.Models
{
    public class GuardedObject
    {
        public int Id { get; set; }
        [Display(Name = "Название")]
        [Required(ErrorMessage = "Введите название")]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; }
        [Display(Name = "Изображение")]

        public string? Image { get; set; }
        [Display(Name = "Город")]
        [StringLength(100, MinimumLength = 1)]
        [Required(ErrorMessage = "Введите город")]
        public string City { get; set; }
        [Display(Name = "Улица")]
        [StringLength(100, MinimumLength = 1)]
        [Required(ErrorMessage = "Введите улицу")]
        public string Street { get; set; }
        [Display(Name = "Здание")]
        [StringLength(100, MinimumLength = 1)]
        [Required(ErrorMessage = "Введите здание")]
        public string Building { get; set; }
    }
}
