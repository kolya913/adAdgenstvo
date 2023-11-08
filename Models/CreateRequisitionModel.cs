using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace adAdgenstvo.Models
{
    public class CreateRequisitionModel
    {
        public string Text { get; set; }

        public DateTime Deadline { get; set; }

        public List<int> SelectedServiceIds { get; set; }
    }
}
