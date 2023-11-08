using System.ComponentModel.DataAnnotations.Schema;

namespace adAdgenstvo.Models
{
    public class ClienEdit
    { 
        public int Id { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string Surname { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string NameCompany { get; set; }
        public string Login { get; set; }
        public string Inn { get; set; }
        public string Email { get; set; }
    }

}
