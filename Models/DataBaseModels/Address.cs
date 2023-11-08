using System.ComponentModel.DataAnnotations.Schema;

namespace adAdgenstvo.Models.DataBaseModels
{
    [Table("address")]
    public class Address
    {
        public int Id { get; set; }
        public int Region {  get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string Status { get; set; }
        public int PriceServiceId { get; set; }
        public PriceService PriceService { get; set; }
    }
}
