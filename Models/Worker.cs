using System.ComponentModel.DataAnnotations.Schema;

namespace adAdgenstvo.Models
{
    [Table("worker")]
    public class Worker : IUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string Surname { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Login { get; set; }
        public string Inn { get; set; }
        public string Email { get; set; }
        public int Salary { get; set; }
        public int IdRole { get; set; }
        public int IdPosition { get; set; }
        public Role Role { get; set; }
        public Position Position { get; set; }
    }

}
