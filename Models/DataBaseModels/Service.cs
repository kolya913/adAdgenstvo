using System.ComponentModel.DataAnnotations.Schema;

namespace adAdgenstvo.Models.DataBaseModels
{
    [Table("service")]
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
    }
}
