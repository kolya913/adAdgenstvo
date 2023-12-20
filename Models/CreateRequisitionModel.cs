using adAdgenstvo.Models.DataBaseModels;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace adAdgenstvo.Models
{
    public class CreateRequisitionModel
    {
        [Required]
        public string? TextField { get; set; }

        [HiddenInput]
        public int? Id { get; set; }

        public List<PriceService>? Services { get; set; }
        [Required]
        public List<int>? SelectedServiceIds { get; set; }
    }
}
