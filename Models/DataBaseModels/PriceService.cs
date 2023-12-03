using adAdgenstvo.Models.CreateModels;
using adAdgenstvo.Models.EditModels;
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
        public Service? Service { get; set; }
    
        public PriceService() { }

        public PriceService(PriceServiceCM priceServiceCM)
        {
            Name = priceServiceCM.Name;
            Price = priceServiceCM.Price;
            Count = priceServiceCM.Count;
            TimeToProduce = priceServiceCM.TimeToProduce;
            ServiceId = priceServiceCM.ServiceId;
        }

        public PriceService Update(PriceServiceEM priceServiceEM)
        {
            Id = priceServiceEM.Id;
            if (priceServiceEM.Name != null)
            {
                Name = priceServiceEM.Name;
            }
            if (priceServiceEM.Price != null)
            {
                Price = (int)priceServiceEM.Price;
            }
            if (priceServiceEM.Count != null)
            {
                Count = (int)priceServiceEM.Count;
            }
            if (priceServiceEM.TimeToProduce != null)
            {
                TimeToProduce = (DateOnly)priceServiceEM.TimeToProduce;
            }
            if (priceServiceEM.ServiceId != null)
            {
                ServiceId = (int)priceServiceEM.ServiceId;
            }
            return this;
        }



    }
}
