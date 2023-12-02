namespace Medicines.DataProcessor
{
    using Medicines.Data;
    using Medicines.Data.Models.Enums;
    using Medicines.DataProcessor.ExportDtos;
    using Newtonsoft.Json;
    using System.Globalization;
    using Trucks.Utilities;

    public class Serializer
    {
        public static string ExportPatientsWithTheirMedicines(MedicinesContext context, string date)
        {
            XmlHelper xmlHelper = new XmlHelper();

            ExportPatientDto[] pacientDtos = context.Patients
                .Where(p => p.PatientsMedicines.Any())
                .ToArray()
                .Select(p => new ExportPatientDto
                {
                    Name = p.FullName,
                    Gender = p.Gender.ToString(),
                    AgeGroup = p.AgeGroup.ToString(),
                    Medicines = p.PatientsMedicines
                    .Select(pm => new ExportMedicineDto
                    {
                        Name = pm.Medicine.Name,
                        Price = pm.Medicine.Price.ToString("f2"),
                        Producer = pm.Medicine.Producer,
                        BestBefore = pm.Medicine.ExpiryDate.ToString("yyyy-MM-dd"),
                        Category = pm.Medicine.Category.ToString()
                    })
                    .OrderByDescending(m => m.BestBefore)
                    .ThenBy(m => m.Price)
                    .ToArray()
                })
                .OrderByDescending(p => p.Medicines.Length)
                .ThenBy(c => c.Name)
                .ToArray();

            return xmlHelper.Serialize(pacientDtos, "Patients");
        }

        public static string ExportMedicinesFromDesiredCategoryInNonStopPharmacies(MedicinesContext context, int medicineCategory)
        {
            var medicines = context
                .Medicines
                .Where(m => (int)m.Category == medicineCategory && m.Pharmacy.IsNonStop == true)
                .ToArray()
                .Select(m => new
                {
                    Name = m.Name,
                    Price = m.Price,
                    Pharmacy = new
                    {
                        Name = m.Pharmacy.Name,
                        PhoneNumber = m.Pharmacy.PhoneNumber
                    }
                })
                .OrderBy(m => m.Price.ToString("f2"))
                .ThenBy(m => m.Name)
                .ToArray();

            return JsonConvert.SerializeObject(medicines, Formatting.Indented);
        }
    }
}
