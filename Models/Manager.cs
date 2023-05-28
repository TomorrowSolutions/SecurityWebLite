using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SecurityLite.Models
{
    public class Manager
    {
        public int Id { get; set; }
        [Display(Name = "Фамилия")]
        [StringLength(100, MinimumLength = 1)]
        public string? Surname { get; set; }
        [Display(Name = "Имя")]
        [Required(ErrorMessage = "Введите имя")]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; }
        [Display(Name = "Отчество")]
        [StringLength(100, MinimumLength = 1)]
        public string? Patronymic { get; set; }
        [Display(Name = "Образование")]
        [Required(ErrorMessage = "Укажите тип образования")]
        public string Education { get; set; }
        [Display(Name = "Категория")]
        public int? CategoryId { get; set; }
        [Display(Name = "Категория")]
        public Category? Category { get; set; }
        [Display(Name = "Дата начала работы")]
        [Required(ErrorMessage = "Укажите дату начала работы")]
        public DateTime DateOfStart { get; set; }
        [Display(Name = "Номер счета")]
        [Required(ErrorMessage = "Введите номер счета")]
        [StringLength(21, MinimumLength = 21, ErrorMessage = "Номер должен состоять из 21 цифр")]
        public string AccountNum { get; set; }
        [NotMapped]
        public string GetFIO { get { return this.Surname+" "+ string.Join(' ', this.Name, this.Patronymic); } }
    }
}
