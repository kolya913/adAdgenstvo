using adAdgenstvo.Models.DataBaseModels;

namespace adAdgenstvo.Models.EditModels
{
    public class PriceServiceEM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? Price { get; set; }
        public int? Count { get; set; }
        public DateOnly? TimeToProduce { get; set; }
        public int? ServiceId { get; set; }
        public Service? Service { get; set; }
        
        public PriceServiceEM(PriceService priceService) 
        {
            Id = priceService.Id;
            Name = priceService.Name;
            Price = priceService.Price;
            Count = priceService.Count;
            TimeToProduce = priceService.TimeToProduce;
            ServiceId = priceService.ServiceId;
            Service = priceService.Service;
        }
    }
}
