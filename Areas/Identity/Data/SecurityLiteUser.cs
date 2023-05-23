using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SecurityLite.Areas.Identity.Data;

// Add profile data for application users by adding properties to the SecurityLiteUser class
public class SecurityLiteUser : IdentityUser
{
    [Display(Name = "Номер счета")]
    [Required(ErrorMessage = "Введите номер счета")]
    [StringLength(21, MinimumLength = 21, ErrorMessage = "Номер должен состоять из 21 цифр")]
    public string AccountNum { get; set; }
    [NotMapped]
    public string RoleName { get; set; }
}

