namespace Medicines.DataProcessor.ExportDtos
{
    public class MedicineExportDTO
    {
        public string Name { get; set; } = null!;
        public string Price { get; set; } = null!;
        public PharmacyExportDTO Pharmacy { get; set; } = null!;
    }
}
