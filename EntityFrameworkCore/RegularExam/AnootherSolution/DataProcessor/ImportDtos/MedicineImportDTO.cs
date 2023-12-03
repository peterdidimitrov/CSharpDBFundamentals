using Castle.Components.DictionaryAdapter;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Medicines.DataProcessor.ImportDtos
{
    [XmlType("Medicine")]
    public class MedicineImportDTO
    {
        [Required]
        [XmlAttribute("category")]
        [Range(0, 4)]
        public int Category { get; set; }

        [Required]
        [MaxLength(150)]
        [MinLength(3)]
        public string Name { get; set; } = null!;

        [Required]
        [Range(0.01, 1000.00)]
        public double Price { get; set; }

        [Required]
        public string ProductionDate { get; set; } = null!;

        [Required]
        public string ExpiryDate { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [MinLength(3)]
        public string Producer { get; set; } = null!;
    }
}
