using System.ComponentModel.DataAnnotations.Schema;

namespace adAdgenstvo.Models.DataBaseModels
{
    [Table("servicetype")]
    public class ServiceType
    {
        public int Id { get; set; }
        public required string ShortName { get; set; }
        public required string Description { get; set; }
        public required string Type { get; set; }
        public string? Status { get; set; }

        public ServiceType() 
        {

        }
    }
}
