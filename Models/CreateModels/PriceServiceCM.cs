namespace adAdgenstvo.Models.CreateModels
{
    public class PriceServiceCM
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public int Count { get; set; }
        public DateOnly TimeToProduce { get; set; }
        public int ServiceId { get; set; }
    }
}
