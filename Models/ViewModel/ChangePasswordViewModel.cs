using System.ComponentModel.DataAnnotations;

namespace SecurityLite.Models.ViewModel
{
    public class ChangePasswordViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Введите пароль")]
        [StringLength(255, MinimumLength = 4, ErrorMessage = "Пароль должен состоять из не менее 4 символов")]
        public string NewPassword { get; set; }
        
    }
}
