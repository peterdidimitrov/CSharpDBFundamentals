namespace Medicines.DataProcessor;

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

            List<PatientMedicine> patientsMedicines = new List<PatientMedicine>();

            foreach (int medicineId in patientDto.Medicines)
            {
                if (patientsMedicines.Select(pm => pm.MedicineId).Contains(medicineId))
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                PatientMedicine patientMedicine = new PatientMedicine()
                {
                    Patient = patient,
                    MedicineId = medicineId
                };

                patientsMedicines.Add(patientMedicine);
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

            if (!IsValid(pharmacyDto) || !bool.TryParse(pharmacyDto.IsNonStop, out bool temp))
            {
                stringBuilder.AppendLine(ErrorMessage);
                continue;
            }


            ICollection<Medicine> validMedicines = new HashSet<Medicine>();

            foreach (ImportMedicineDto medicineDto in pharmacyDto.Medicines)
            {

                bool nameAneProducerExistCurrent = validMedicines.Select(md => new { md.Name, md.Producer }).Any(md => md.Name == medicineDto.Name && md.Producer == medicineDto.Producer);


                if (!IsValid(medicineDto)
                    || DateTime.ParseExact(medicineDto.ProductionDate, "yyyy-MM-dd", CultureInfo.InvariantCulture) >= DateTime.ParseExact(medicineDto.ExpiryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                    || nameAneProducerExistCurrent)
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                Medicine medicine = new Medicine()
                {
                    Name = medicineDto.Name,
                    Price = (decimal)medicineDto.Price,
                    ProductionDate = DateTime.ParseExact(medicineDto.ProductionDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    ExpiryDate = DateTime.ParseExact(medicineDto.ExpiryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Producer = medicineDto.Producer,
                    Category = (Category)medicineDto.Category
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
