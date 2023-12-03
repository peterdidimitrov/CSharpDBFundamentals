using System.Xml.Serialization;

namespace Medicines.DataProcessor.ExportDtos
{
    [XmlType("Patient")]
    public class PatientExportDTO
    {
        [XmlAttribute]
        public string Gender { get; set; } = null!;

        [XmlElement]
        public string Name { get; set; } = null!;

        [XmlElement]
        public string AgeGroup { get; set; } = null!;

        [XmlArray]
        [XmlArrayItem("Medicine")]
        public MedicineExportXmlDTO[] Medicines { get; set; } = null!;
    }
}
