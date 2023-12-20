using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using adAdgenstvo.Models.DataBaseModels;

namespace adAdgenstvo.Models.DataBaseModels
{
    [Table("project")]
    public class LayoutProject
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string? Text { get; set; }

        public List<LayoutPhoto>? Photos { get; set; }
    }
}
