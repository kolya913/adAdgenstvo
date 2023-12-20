using System.ComponentModel.DataAnnotations;

namespace adAdgenstvo.Models.LoginModels
{
    public class UserLM
    {
        [Required(ErrorMessage = "Адрес почты обязателен")]
        [EmailAddress(ErrorMessage = "Адрес почты не правильный")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль оябзателен")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
