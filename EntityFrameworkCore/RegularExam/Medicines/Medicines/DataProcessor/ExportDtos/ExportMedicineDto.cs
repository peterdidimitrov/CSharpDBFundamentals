using Medicines.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Medicines.DataProcessor.ExportDtos
{
    [XmlType("Medicine")]
    public class ExportMedicineDto
    {
        [XmlElement("Name")]
        public string Name { get; set; } = null!;

        [XmlAttribute("Category")]
        public string Category { get; set; }

        [XmlElement("Price")]
        public string Price { get; set; }

        [XmlElement("Producer")]
        public string Producer { get; set; } = null!;

        [XmlElement("BestBefore")]
        public string BestBefore { get; set; }
    }
}
