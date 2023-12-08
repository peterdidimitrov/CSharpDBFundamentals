using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftJail.DataProcessor.ImportDto
{
    public class ImportMailDto
    {
        [JsonProperty("Description")]
        [Required]
        public string Description { get; set; }

        [JsonProperty("Sender")]
        [Required]
        public string Sender { get; set; }

        [JsonProperty("Address")]
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9\s]+str\.$")]
        public string Address { get; set; }
    }
}