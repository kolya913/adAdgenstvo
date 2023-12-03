namespace adAdgenstvo.Models.CreateModels;

public class AddressCM {
    public required int Region {  get; set; }
    public required string City { get; set; }
    public required string Street { get; set; }
    public required string House { get; set; }
    public required string Status { get; set; }
    public required int PriceServiceId { get; set; }
}