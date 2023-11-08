using System.ComponentModel.DataAnnotations.Schema;

namespace adAdgenstvo.Models
{
    [Table("order_service")]
    public class OrderService
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public int OrderId { get; set; }
        public string? IdentifierAd { get; set; }
        public string Status { get; set; }
        public Service Service { get; set; }
        public Order Order { get; set; }
    }

}
