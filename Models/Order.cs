using System.ComponentModel.DataAnnotations.Schema;

namespace adAdgenstvo.Models
{
    [Table("order")]
    public class Order
    {
        public int Id { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime? DateCreate {get; set;}
        public int ClientId { get; set; }
        public int? AgentId { get; set; }
        public string Status { get; set; }
        public Client Client { get; set; }
        public Worker Agent { get; set; }
        public string? Text {get; set;}
        public string? DebateText { get; set;} 
    }

}
