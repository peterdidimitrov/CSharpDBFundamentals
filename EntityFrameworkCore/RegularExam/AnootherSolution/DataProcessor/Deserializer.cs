namespace Medicines.DataProcessor
{
    using Boardgames.Extensions;
    using Medicines.Data;
    using Medicines.Data.Models;
    using Medicines.DataProcessor.ImportDtos;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid Data!";
        private const string SuccessfullyImportedPharmacy = "Successfully imported pharmacy - {0} with {1} medicines.";
        private const string SuccessfullyImportedPatient = "Successfully imported patient - {0} with {1} medicines.";

        public static string ImportPatients(MedicinesContext context, string jsonString)
        {
            PatientImportDTO[] patientImportDTOs = JsonConvert.DeserializeObject<PatientImportDTO[]>(jsonString);
            StringBuilder sb = new StringBuilder();
            List<Patient> patients = new List<Patient>();

            foreach (var patient in patientImportDTOs)
            {
                if (!IsValid(patient))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Patient addPatient = new Patient
                {
                    FullName = patient.FullName,
                    AgeGroup = (AgeGroup)patient.AgeGroup,
                    Gender = (Gender)patient.Gender
                };
                List<PatientMedicine> patientsMedicines = new List<PatientMedicine>();

                foreach (int id in patient.Medicines)
                {
                    if (patientsMedicines.Select(x => x.MedicineId).Contains(id))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    patientsMedicines.Add(new PatientMedicine
                    {
                        MedicineId = id,
                        Patient = addPatient
                    });
                }

                addPatient.PatientsMedicines = patientsMedicines;
                patients.Add(addPatient);
                sb.AppendLine(string.Format(SuccessfullyImportedPatient, patient.FullName, patientsMedicines.Count()));
            }

            context.Patients.AddRange(patients);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportPharmacies(MedicinesContext context, string xmlString)
        {
            PharmacyImportDTO[] pharmacyImportDTOs = xmlString.DeserializeFromXml<PharmacyImportDTO[]>("Pharmacies");
            StringBuilder sb = new StringBuilder();
            List<Pharmacy> pharmacies = new List<Pharmacy>();

            foreach (var ph in pharmacyImportDTOs)
            {
                if (!IsValid(ph) || !bool.TryParse(ph.IsNonStop, out bool temp))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Pharmacy addPharmacy = new Pharmacy
                {
                    Name = ph.Name,
                    PhoneNumber = ph.PhoneNumber,
                    IsNonStop = bool.Parse(ph.IsNonStop)
                };
                List<Medicine> medicinesImport = new List<Medicine>();
                foreach (var med in ph.Medicines)
                {
                    bool nameAneProducerExistCurrent = medicinesImport.Select(md => new { md.Name, md.Producer }).Any(md => md.Name == med.Name && md.Producer == med.Producer);

                    if (!IsValid(med)
                        || DateTime.ParseExact(med.ProductionDate, "yyyy-MM-dd", CultureInfo.InvariantCulture) >= DateTime.ParseExact(med.ExpiryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                        || nameAneProducerExistCurrent)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    medicinesImport.Add(new Medicine
                    {
                        Name = med.Name,
                        Price = (decimal)med.Price,
                        ProductionDate = DateTime.ParseExact(med.ProductionDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        ExpiryDate = DateTime.ParseExact(med.ExpiryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        Producer = med.Producer,
                        Category = (Category)med.Category
                    });
                }

                addPharmacy.Medicines = medicinesImport;
                pharmacies.Add(addPharmacy);
                sb.AppendLine(string.Format(SuccessfullyImportedPharmacy, ph.Name, medicinesImport.Count()));
            }

            context.Pharmacies.AddRange(pharmacies);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
