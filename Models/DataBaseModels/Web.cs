using System.ComponentModel.DataAnnotations.Schema;

namespace adAdgenstvo.Models.DataBaseModels
{
    [Table("web")]
    public class Web
    {
        public int Id { get; set; }
        public string URL { get; set; }
        public string Name { get; set; }
        public string Platform { get; set; }
        public string Status { get; set; }
        public int PriceServiceId { get; set; }
        public PriceService PriceService { get; set; }
    }
}
