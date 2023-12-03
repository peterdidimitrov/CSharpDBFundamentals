namespace Medicines.DataProcessor
{
    using Boardgames.Extensions;
    using Medicines.Data;
    using Medicines.DataProcessor.ExportDtos;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using System.Globalization;

    public class Serializer
    {
        public static string ExportPatientsWithTheirMedicines(MedicinesContext context, string date)
        {
            DateTime producedFrom = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            PatientExportDTO[] patientExportDTOs = context.Patients
                .Where(p => p.PatientsMedicines.Any(pm => pm.Medicine.ProductionDate > producedFrom))
                .Select(p => new PatientExportDTO
                {
                    Name = p.FullName,
                    AgeGroup = p.AgeGroup.ToString(),
                    Gender = p.Gender.ToString().ToLower(),
                    Medicines = p.PatientsMedicines
                    .Where(pm => pm.Medicine.ProductionDate > producedFrom)
                    .OrderByDescending(pm => pm.Medicine.ExpiryDate)
                        .ThenBy(pm => pm.Medicine.Price)
                    .Select(pm => new MedicineExportXmlDTO
                    {
                        Category = pm.Medicine.Category.ToString().ToLower(),
                        Name = pm.Medicine.Name,
                        Price = pm.Medicine.Price.ToString("f2"),
                        Producer = pm.Medicine.Producer,
                        BestBefore = pm.Medicine.ExpiryDate.ToString("yyyy-MM-dd")
                    })
                    .ToArray()
                })
                .OrderByDescending(p => p.Medicines.Count())
                    .ThenBy(p => p.Name)
                .ToArray();

            string xmlified = patientExportDTOs.SerializeToXml("Patients");

            return xmlified;
        }

        public static string ExportMedicinesFromDesiredCategoryInNonStopPharmacies(MedicinesContext context, int medicineCategory)
        {
            MedicineExportDTO[] medicineExportDTOs = context.Medicines
                .Where(m => m.Category == (Category)medicineCategory && m.Pharmacy.IsNonStop)
                .OrderBy(m => m.Price)
                    .ThenByDescending(m => m.Name)
                .Select(m => new MedicineExportDTO
                {
                    Name = m.Name,
                    Price = m.Price.ToString("f2"),
                    Pharmacy = new PharmacyExportDTO
                    {
                        Name = m.Pharmacy.Name,
                        PhoneNumber = m.Pharmacy.PhoneNumber
                    }
                })
                .AsNoTracking()
                .ToArray();

            string stringyfied = JsonConvert.SerializeObject(medicineExportDTOs, Formatting.Indented);

            return stringyfied;
        }
    }
}
