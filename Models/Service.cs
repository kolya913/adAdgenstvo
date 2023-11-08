using System.ComponentModel.DataAnnotations.Schema;

namespace adAdgenstvo.Models
{
    [Table("service")]
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string Status { get; set; }
    }

}
