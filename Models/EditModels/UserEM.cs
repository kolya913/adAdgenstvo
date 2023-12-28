using adAdgenstvo.Models.DataBaseModels;
using System.ComponentModel.DataAnnotations;

namespace adAdgenstvo.Models.EditModels
{
    public class UserEM
    {

        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Patronymic { get; set; }

        public string? Lastname { get; set; }

        [DataType(DataType.Password)]
        [StringLength(64, MinimumLength = 6, ErrorMessage = "Пароль от 6 до 64 символов")]
        public string? Password { get; set; }

        [EmailAddress(ErrorMessage = "Неправильный адрес")]
        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Inn { get; set; }

        public string? NameCompany { get; set; }

        public int? RoleId { get; set; }

        public int? PositionId { get; set; }

        public Position? Position { get; set; }

        public UserEM() { }

        public UserEM(User user)
        {
            Id = user.Id;
            Name = user.Name;   
            Patronymic = user.Patronymic;
            Lastname = user.Lastname;
            Email = user.Email;
            PhoneNumber = user.PhoneNumber;
            NameCompany = user.NameCompany;
            RoleId = user.RoleId;
            PositionId = user.PositionId;
            Inn = user.Inn;
            Position = user.Position;
        }

    }
}
