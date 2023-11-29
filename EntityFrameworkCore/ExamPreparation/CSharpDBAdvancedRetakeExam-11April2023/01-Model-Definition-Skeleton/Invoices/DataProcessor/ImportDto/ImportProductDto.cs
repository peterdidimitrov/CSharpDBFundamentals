using Invoices.Data.Models.Enums;
using Invoices.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Invoices.DataProcessor.ImportDto
{
    public class ImportProductDto
    {
        [JsonProperty("Name")]
        [Required]
        [MinLength(9)]
        [MaxLength(30)]
        public string Name { get; set; } = null!;

        [JsonProperty("Price")]
        [Required]
        [Range(5.00, 1000.00)]
        public decimal Price { get; set; }

        [JsonProperty("CategoryType")]
        [Required]
        public CategoryType CategoryType { get; set; }

        [JsonProperty("Clients")]
        public int[] ClientIds { get; set; } = null!;
    }
}