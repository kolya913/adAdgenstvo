using adAdgenstvo.Models.CreateModels;
using adAdgenstvo.Models.EditModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace adAdgenstvo.Models.DataBaseModels
{
    [Table("service")]
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public int ServiceTypeId { get; set; }
        public ServiceType? ServiceType { get; set; }

        public Service()
        {

        }

        public Service (ServiceCM serviceCM)
        {
            Name = serviceCM.Name;
            ShortDescription = serviceCM.ShortDescription;
            ServiceTypeId = serviceCM.ServiceTypeId;
        }

        public Service Update(ServiceEM serviceEM)
        {
            Id = serviceEM.Id;
            if(serviceEM.ShortDescription != null)
            {
                ShortDescription = serviceEM.ShortDescription;
            }
            if(serviceEM.Name != null)
            {
                Name = serviceEM.Name;
            }
            if (serviceEM.ServiceTypeId != null)
            {
                Name = serviceEM.Name;
            }
            if (serviceEM.ServiceType != null)
            {
                ServiceType = serviceEM.ServiceType;
            }
            return this;
        }

    }
}
