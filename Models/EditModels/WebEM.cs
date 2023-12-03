using adAdgenstvo.Models.DataBaseModels;

namespace adAdgenstvo.Models.EditModels
{
    public class WebEM
    {
        public int Id { get; set; }
        public string? URL { get; set; }
        public string? Name { get; set; }
        public string? Platform { get; set; }
        public string? Status { get; set; }
        public int? PriceServiceId { get; set; }
        public PriceService? PriceService { get; set; }

        public WebEM(Web web) 
        {
            Id = web.Id;
            URL = web.URL;
            Name = web.Name;
            Platform = web.Platform;
            Status = web.Status;
            PriceServiceId = web.PriceServiceId;
            PriceService = web.PriceService;
        }
    }
}
