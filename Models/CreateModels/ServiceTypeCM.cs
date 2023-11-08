namespace adAdgenstvo.Models.CreateModels
{
    public class ServiceTypeCM
    {
        public required string ShortName { get; set; }
        public required string Description { get; set; }
        public required string Type { get; set; }
        public string? Status { get; set; }

    }
}
