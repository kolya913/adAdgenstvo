using System.ComponentModel.DataAnnotations.Schema;

namespace adAdgenstvo.Models.DataBaseModels
{
    [Table("priceservice")]
    public class PriceService
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int Count { get; set; }
        public DateOnly TimeToProduce { get; set; }
        public int ServiceId { get; set; }
        public Service Service { get; set; }
    }
}
