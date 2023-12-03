using System.ComponentModel.DataAnnotations.Schema;
using adAdgenstvo.Models.CreateModels;
using adAdgenstvo.Models.EditModels;

namespace adAdgenstvo.Models.DataBaseModels
{
    [Table("web")]
    public class Web
    {
        public int Id { get; set; }
        public string URL { get; set; }
        public string Name { get; set; }
        public string Platform { get; set; }
        public string? Status { get; set; }
        public int? PriceServiceId { get; set; }
        public PriceService? PriceService { get; set; }

        public Web()
        {
        }

        public Web(WebCM webCM)
        {
            URL = webCM.URL;
            Name = webCM.Name;
            Platform = webCM.Platform;
            Status = webCM.Status;
            PriceServiceId = webCM.PriceServiceId;
        }

        public Web Update(WebEM webCM)
        {
            Id = webCM.Id;
            if(webCM.URL != null)
            {
                URL = webCM.URL;
            }
            if(webCM.Name != null)
            {
                Name = webCM.Name;
            }
            if(webCM.Platform != null)
            {
                Platform = webCM.Platform;
            }
            if (webCM.Status != null)
            {
                Status = webCM.Status;
            }
            if (webCM.PriceServiceId != null)
            {
                PriceServiceId = webCM.PriceServiceId;
            }
            if (webCM.PriceService != null)
            {
                PriceService = webCM.PriceService;
            }
            return this;
        }

    }
}
