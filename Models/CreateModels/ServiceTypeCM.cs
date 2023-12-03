using System.ComponentModel.DataAnnotations;

namespace adAdgenstvo.Models.CreateModels
{
    public class ServiceTypeCM
    {
        [Required(ErrorMessage = "Обязательно")]
        public string ShortName { get; set; }

        [Required(ErrorMessage = "Обязательно")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Обязательно")]
        public string Type { get; set; }
    }
}
