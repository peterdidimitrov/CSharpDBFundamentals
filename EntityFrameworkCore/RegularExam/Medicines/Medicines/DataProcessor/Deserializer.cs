namespace Medicines.DataProcessor
{
    using Medicines.Data;
    using Medicines.Data.Models;
    using Medicines.Data.Models.Enums;
    using Medicines.DataProcessor.ImportDtos;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using Trucks.Utilities;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid Data!";
        private const string SuccessfullyImportedPharmacy = "Successfully imported pharmacy - {0} with {1} medicines.";
        private const string SuccessfullyImportedPatient = "Successfully imported patient - {0} with {1} medicines.";

        public static string ImportPatients(MedicinesContext context, string jsonString)
        {
            StringBuilder stringBuilder = new StringBuilder();

            ImportPatientDto[] patientDtos = JsonConvert.DeserializeObject<ImportPatientDto[]>(jsonString);

            ICollection<Patient> validPatients = new HashSet<Patient>();

            ICollection<int> existingMedicineIds = context.Medicines
                .Select(m => m.Id)
                .ToArray();

            foreach (ImportPatientDto patientDto in patientDtos)
            {
                if (!IsValid(patientDto))
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                Patient patient = new Patient()
                {
                    FullName = patientDto.FullName,
                    AgeGroup = (AgeGroup)patientDto.AgeGroup,
                    Gender = (Gender)patientDto.Gender
                };

                foreach (int medicineId in patientDto.Medicines.Distinct())
                {
                    if (!existingMedicineIds.Contains(medicineId))
                    {
                        stringBuilder.AppendLine(ErrorMessage);
                        continue;
                    }

                    PatientMedicine patientMedicine = new PatientMedicine()
                    {
                        Patient = patient,
                        MedicineId = medicineId
                    };
                    patient.PatientsMedicines.Add(patientMedicine);
                }
                validPatients.Add(patient);
                stringBuilder.AppendLine(string.Format(SuccessfullyImportedPatient, patient.FullName, patient.PatientsMedicines.Count));
            }

            context.Patients.AddRange(validPatients);
            context.SaveChanges();

            return stringBuilder.ToString().Trim();
        }

        public static string ImportPharmacies(MedicinesContext context, string xmlString)
        {
            StringBuilder stringBuilder = new StringBuilder();
            XmlHelper xmlHelper = new XmlHelper();

            ImportPharmacyDto[] pharmacyDtos = xmlHelper.Deserialize<ImportPharmacyDto[]>(xmlString, "Pharmacies");

            ICollection<Pharmacy> validPharmacies = new HashSet<Pharmacy>();

            foreach (ImportPharmacyDto pharmacyDto in pharmacyDtos)
            {

                if (!IsValid(pharmacyDto))
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                bool isBool;
                bool isBoolValid = bool.TryParse(pharmacyDto.IsNonStop, out isBool);

                if (!isBoolValid)
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                ICollection<Medicine> validMedicines = new HashSet<Medicine>();

                foreach (ImportMedicineDto medicineDto in pharmacyDto.Medicines)
                {
                    if (!IsValid(medicineDto) || medicineDto.Producer == null)
                    {
                        stringBuilder.AppendLine(ErrorMessage);
                        continue;
                    }
                    if (validMedicines.Any(m => m.Name == medicineDto.Name || m.Producer == medicineDto.Producer))
                    {
                        stringBuilder.AppendLine(ErrorMessage);
                        continue;
                    }


                    DateTime medicineProductionDate;
                    bool isMedicineProductionDateValid = DateTime.TryParseExact(medicineDto.ProductionDate,
                        "yyyy-MM-dd", CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out medicineProductionDate);
                    if (!isMedicineProductionDateValid)
                    {
                        stringBuilder.AppendLine(ErrorMessage);
                        continue;
                    }

                    DateTime medicineExpiryDate;
                    bool isMedicineExpiryDateValid = DateTime.TryParseExact(medicineDto.ExpiryDate,
                        "yyyy-MM-dd", CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out medicineExpiryDate);

                    if (!isMedicineExpiryDateValid)
                    {
                        stringBuilder.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (medicineProductionDate >= medicineExpiryDate)
                    {
                        stringBuilder.AppendLine(ErrorMessage);
                        continue;
                    }

                    Medicine medicine = new Medicine()
                    {
                        Name = medicineDto.Name,
                        Category = (Category)medicineDto.Category,
                        Price = medicineDto.Price,
                        ProductionDate = medicineProductionDate,
                        ExpiryDate = medicineExpiryDate,
                        Producer = medicineDto.Producer
                    };

                    validMedicines.Add(medicine);
                }

                Pharmacy pharmacy = new Pharmacy()
                {
                    Name = pharmacyDto.Name,
                    IsNonStop = bool.Parse(pharmacyDto.IsNonStop),
                    PhoneNumber = pharmacyDto.PhoneNumber,
                    Medicines = validMedicines
                };
                validPharmacies.Add(pharmacy);
                stringBuilder.AppendLine(string.Format(SuccessfullyImportedPharmacy, pharmacy.Name, pharmacy.Medicines.Count));
            }

            context.Pharmacies.AddRange(validPharmacies);
            context.SaveChanges();

            return stringBuilder.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
