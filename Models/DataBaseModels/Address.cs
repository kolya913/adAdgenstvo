using System.ComponentModel.DataAnnotations.Schema;
using adAdgenstvo.Models.CreateModels;
using adAdgenstvo.Models.EditModels;

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

        public Address()
        {
        }

        public Address(AddressCM addressCM)
        {
            Region = addressCM.Region;
            City = addressCM.City;
            Street = addressCM.Street;
            House = addressCM.House;
            Status  = addressCM.Status;
            PriceServiceId = addressCM.PriceServiceId;
        }

        public Address(AddressEM addressEM)
        {
            Id = addressEM.Id;
            if(addressEM.Region != null)
            {
                Region = (int) addressEM.Region;
            }
            if(addressEM.City != null)
            {
                City = addressEM.City;
            }
            if(addressEM.Street != null)
            {
                Street = addressEM.Street;
            }
            if(addressEM.House != null)
            {
                House = addressEM.House;
            }
            if(addressEM.Status != null)
            {
                Status = addressEM.Status;
            }
            if(addressEM.PriceServiceId != null)
            {
                PriceServiceId = (int) addressEM.PriceServiceId;
            }
        }

    }
}
