using System.ComponentModel.DataAnnotations.Schema;

namespace adAdgenstvo.Models.DataBaseModels
{
    [Table("orderpriceservice")]
    public class OrderServicePrice
    {
        public int Id { get; set; }
        public int PriceServiceId { get; set; }
        public int OrderId { get; set; }
        public PriceService? PriceService { get; set; }
    }
}
