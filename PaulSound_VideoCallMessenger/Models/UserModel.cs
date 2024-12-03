using System.ComponentModel.DataAnnotations;

namespace PaulSound_VideoCallMessenger.Models
{
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }
        [Required(ErrorMessage = "Требуется логин")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Username must be 3-20 characters long")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Логин может содержать только символы латинского алфавита и числа")]
        public string? UserName { get; set; }
        [Required(ErrorMessage = "Требуется email")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$",
          ErrorMessage = "Пожалуйста введите валидный email адрес")]
        [MaxLength(100,ErrorMessage ="Email превышает 100 символов")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Требуется пароль")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
          ErrorMessage = "Пароль должен содержать минимум 8 символов иметь хотябы один символ верхнего регистра " +
          "и один символ нижнего регистра, число и специальный символ!")]

        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [Required(ErrorMessage = "Пожалуйста подтвердите пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string? ConfirmPassword { get; set; }
    }
}
