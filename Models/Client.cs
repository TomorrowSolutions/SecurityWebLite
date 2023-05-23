using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Xml.Linq;

namespace SecurityLite.Models
{
    public class Client
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
        [Display(Name = "Номер телефона")]
        [Required(ErrorMessage = "Введите номер телефона")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Номер должен состоять из 11 цифр")]
        public string PhoneNum { get; set; }
        [Display(Name = "Тип лица")]
        [Required(ErrorMessage = "Укажите тип лица")]
        public string TypeOfPerson { get; set; }
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
        [Display(Name = "Номер счета")]
        [Required(ErrorMessage = "Введите номер счета")]
        [StringLength(21, MinimumLength = 21, ErrorMessage = "Номер должен состоять из 21 цифр")]
        public string AccountNum { get; set; }
        [NotMapped]
        public string GetFIO { get { return this.Surname+" "+ string.Join(' ', this.Name, this.Patronymic); } }
    }
}
