using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftJail.DataProcessor.ImportDto
{
    public class ImportCellDto
    {
        [JsonProperty("CellNumber")]
        [Required]
        [Range(1, 1000)]
        public int CellNumber { get; set; }

        [JsonProperty("HasWindow")]
        [Required]
        public bool HasWindow { get; set; }
    }
}