using System.ComponentModel.DataAnnotations.Schema;

namespace adAdgenstvo.Models.DataBaseModels
{
    [Table("position")]
    public class Position
    {   
        public int Id { get; set; }
        public required string PositionName { get; set; }
    }
}
