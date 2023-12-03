using adAdgenstvo.Models.CreateModels;
using adAdgenstvo.Models.EditModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace adAdgenstvo.Models.DataBaseModels
{
    [Table("servicetypes")]
    public class ServiceType
    {
        public int Id { get; set; }
        public  string ShortName { get; set; }
        public  string Description { get; set; }
        public  string Type { get; set; }
        public string? Status { get; set; }

        public ServiceType() 
        {

        }
        
        public ServiceType(ServiceTypeCM serviceTypeCM)
        {
            ShortName = serviceTypeCM?.ShortName ?? throw new ArgumentNullException(nameof(serviceTypeCM), "ServiceTypeCM must not be null");
            Description = serviceTypeCM?.Description ?? throw new ArgumentNullException(nameof(serviceTypeCM), "ServiceTypeCM must not be null");
            Type = serviceTypeCM?.Type ?? throw new ArgumentNullException(nameof(serviceTypeCM), "ServiceTypeCM must not be null");
            Status = "Hidden";
        }

        public ServiceType Update(ServiceTypeEM serviceTypeEM)
        {
            Id = serviceTypeEM.Id;
            if(serviceTypeEM.Description != null)
            {
                Description = serviceTypeEM.Description;
            }
            if(serviceTypeEM.Name != null)
            {
                ShortName = serviceTypeEM.Name;
            }
            return this;
        }

    }
}
