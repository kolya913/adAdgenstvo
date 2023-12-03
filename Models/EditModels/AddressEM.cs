namespace adAdgenstvo.Models.EditModels;

public class AddressEM {
    public int Id { get; set; }
    public int? Region {  get; set; }
    public string? City { get; set; }
    public string? Street { get; set; }
    public string? House { get; set; }
    public string? Status { get; set; }
    public int? PriceServiceId { get; set; }
}