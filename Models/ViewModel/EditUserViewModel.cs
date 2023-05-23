using System.ComponentModel.DataAnnotations;

namespace SecurityLite.Models.ViewModel
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        [Display(Name = "Электронная почта")]
        [Required(ErrorMessage = "Введите адрес электронной почты")]
        public string Email { get; set; }
        [Display(Name = "Номер счета")]
        [Required(ErrorMessage = "Введите номер счета")]
        [StringLength(21, MinimumLength = 21, ErrorMessage = "Номер должен состоять из 21 цифр")]
        public string AccNum { get; set; }
    }
}
