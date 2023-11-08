using System.ComponentModel.DataAnnotations.Schema;

namespace adAdgenstvo.Models
{
    [Table("role")]
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

}
