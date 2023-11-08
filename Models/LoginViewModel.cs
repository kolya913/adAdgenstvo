using System.ComponentModel.DataAnnotations;

namespace adAdgenstvo.Models
{
    public class LoginViewModel
    {
        public string Login { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
