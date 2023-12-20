using adAdgenstvo.Models.DataBaseModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace adAdgenstvo.Models.DataBaseModels
{
    [Table("contract")]
    public class Contract
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int Price { get; set; }
        public DateTime DateStartCompany { get; set; }
        public DateTime DateEndCompany { get; set; }
        public string? Status { get; set; }
        public Order? Order { get; set; }
    }

}
