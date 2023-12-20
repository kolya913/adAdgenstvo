using System.ComponentModel.DataAnnotations.Schema;

namespace adAdgenstvo.Models.DataBaseModels
{
    [Table("order")]
    public class Order
    {
        public int Id { get; set; }
        [Column(TypeName = "date")]
        public DateOnly? DateCreate { get; set; }
        public int ClientId { get; set; }
        public int? AgentId { get; set; }
        public string? Status { get; set; }
        public User? Client { get; set; }
        public User? Agent { get; set; }
        public string? Text { get; set; }
    }
}
