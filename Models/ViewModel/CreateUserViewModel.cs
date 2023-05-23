using System.ComponentModel.DataAnnotations;

namespace SecurityLite.Models.ViewModel
{
    public class CreateUserViewModel
    {
        [Display(Name = "Электронная почта")]
        [Required(ErrorMessage = "Введите адрес электронной почты")]
        public string Email { get; set; }
        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Введите пароль")]
        [StringLength(255,MinimumLength = 4, ErrorMessage = "Пароль должен состоять из не менее 4 символов")]
        public string Password { get; set; }
        [Display(Name = "Номер счета")]
        [Required(ErrorMessage = "Введите номер счета")]
        [StringLength(21, MinimumLength = 21, ErrorMessage = "Номер должен состоять из 21 цифр")]
        public string AccNum { get; set; }
    }
}
