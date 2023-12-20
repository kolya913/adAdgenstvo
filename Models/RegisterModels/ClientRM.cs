using System.ComponentModel.DataAnnotations;

namespace adAdgenstvo.Models.RegisterModel
{
    public class ClientRM
    {
        [Required(ErrorMessage = "Адрес почты обязателен")]
        [EmailAddress(ErrorMessage = "Неправильный адрес")]
        public required string Email { get; set; }

        [Required]
        [StringLength(64, MinimumLength = 6, ErrorMessage = "Пароль от 6 до 64 символов")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,64}$", ErrorMessage = "Пароль состоит из латинских букв, цифр и спецзнаков. Как минимум должна быть одна заглавная буква и один спецзнак.")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
        
        [Required(ErrorMessage = "Номер телефона обязателен")]
        public required string PhoneNumber { get; set; }
        
        [Required(ErrorMessage = "Имя обязательно")]
        [RegularExpression(@"^[a-zA-Zа-яА-Я]+$", ErrorMessage = "Имя должно состоять только из кириллических или латинских букв.")]
        public required string Name { get; set; }
        
        public string? NameCompany { get; set; }
        
        [RegularExpression(@"^[a-zA-Zа-яА-Я]+$", ErrorMessage = "Отчество должно состоять только из кириллических или латинских букв.")]
        public string? Patronymic { get; set; }
        
        [Required(ErrorMessage = "Фамилия обязательна")]
        [RegularExpression(@"^[a-zA-Zа-яА-Я]+$", ErrorMessage = "Фамилия должно состоять только из кириллических или латинских букв.")]
        public required string LastName { get; set; }
       
        [StringLength(12, MinimumLength = 10, ErrorMessage = "ИНН должен содержать от 10 до 12 цифр.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "ИНН должен состоять только из цифр.")]
        public virtual string? Inn { get; set; }

        public int? PositionId { get; set; }
    }
}
