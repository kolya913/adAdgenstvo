using System.ComponentModel.DataAnnotations.Schema;

namespace adAdgenstvo.Models
{
    [Table("contract")]
    public class Contract
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int Cost { get; set; }
        public int Price { get; set; }
        public DateTime DateStartCompany { get; set; }
        public DateTime DateEndCompany { get; set; }
        public DateTime DateSign { get; set; }
        public string Status { get; set; }
        public int CostExecutor { get; set; }
        public string TextContract { get; set; }
        public Order Order { get; set; }
    }

}
