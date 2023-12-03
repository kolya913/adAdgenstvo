using adAdgenstvo.Models.DataBaseModels;

namespace adAdgenstvo.Models.EditModels
{
    public class ServiceTypeEM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }

        public ServiceTypeEM() { }

        public ServiceTypeEM(ServiceType serviceType)
        {
            Id = serviceType.Id;
            Name = serviceType.ShortName;
            Description = serviceType.Description;
            Type = serviceType.Type;
        }
    }
}
