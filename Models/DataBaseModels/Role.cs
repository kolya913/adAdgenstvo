using System.ComponentModel.DataAnnotations.Schema;

namespace adAdgenstvo.Models.DataBaseModels
{
    [Table("role")]
    public class Role
    {
        public int Id { get; set; }
        public required string RoleName { get; set; }
    }
}
