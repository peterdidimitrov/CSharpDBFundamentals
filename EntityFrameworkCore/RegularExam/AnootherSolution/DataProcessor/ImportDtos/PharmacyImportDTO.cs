using Castle.Components.DictionaryAdapter;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Medicines.DataProcessor.ImportDtos
{
    [XmlType("Pharmacy")]
    public class PharmacyImportDTO
    {
        [XmlAttribute("non-stop")]
        public string IsNonStop { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        [MinLength(2)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(14)]
        [MinLength(14)]
        [RegularExpression(@"\(\d{3}\) \d{3}\-\d{4}")]
        public string PhoneNumber { get; set; } = null!;

        [XmlArray]
        [XmlArrayItem("Medicine")]
        public MedicineImportDTO[] Medicines { get; set; } = null!;
    }
}
