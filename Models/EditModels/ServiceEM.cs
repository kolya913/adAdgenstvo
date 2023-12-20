using adAdgenstvo.Models.DataBaseModels;

namespace adAdgenstvo.Models.EditModels
{
    public class ServiceEM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? ShortDescription { get; set; }
        public int? ServiceTypeId { get; set; }
        public ServiceType? ServiceType { get; set; }
        public ServiceEM() { }

        public ServiceEM(Service service) 
        {
            Id = service.Id;
            Name = service.Name;
            ShortDescription = service.ShortDescription;
            ServiceTypeId = service.ServiceTypeId;
            ServiceType = service.ServiceType;

        }
    }
}
