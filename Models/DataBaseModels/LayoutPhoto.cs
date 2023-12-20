using System.ComponentModel.DataAnnotations.Schema;

namespace adAdgenstvo.Models.DataBaseModels
{
    [Table("photo")]
    public class LayoutPhoto 
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        [ForeignKey("ProjectId")]
        public int ProjectId { get; set; }
        public LayoutProject? Project { get; set; }
        public bool IsImage { get; set; }
        public bool IsVideo { get; set; }
    }
}
