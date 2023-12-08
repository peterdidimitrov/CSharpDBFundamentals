namespace SoftJail.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;
    using Trucks.Utilities;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid Data";

        private const string SuccessfullyImportedDepartment = "Imported {0} with {1} cells";

        private const string SuccessfullyImportedPrisoner = "Imported {0} {1} years old";

        private const string SuccessfullyImportedOfficer = "Imported {0} ({1} prisoners)";

        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            StringBuilder stringBuilder = new StringBuilder();

            ImportDepartmentDto[] departmentDtos = JsonConvert.DeserializeObject<ImportDepartmentDto[]>(jsonString);

            ICollection<Department> validDepartments = new HashSet<Department>();


            foreach (ImportDepartmentDto departmentDto in departmentDtos)
            {

                if (!IsValid(departmentDto) || departmentDto.Name.ToLower() == "invalid" || departmentDto.Cells.Length <= 0)
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                Department department = new Department()
                {
                    Name = departmentDto.Name
                };

                bool isCellNumberValid = true;

                foreach (ImportCellDto cellDto in departmentDto.Cells)
                {

                    if (cellDto.CellNumber < 1 || cellDto.CellNumber > 1000)
                    {
                        isCellNumberValid = false;
                        continue;
                    }

                    if (!IsValid(cellDto))
                    {
                        stringBuilder.AppendLine(ErrorMessage);
                        continue;
                    }



                    Cell cell = new Cell()
                    {
                        CellNumber = cellDto.CellNumber,
                        HasWindow = cellDto.HasWindow
                    };

                    department.Cells.Add(cell);
                }

                if (isCellNumberValid == false)
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                validDepartments.Add(department);
                stringBuilder.AppendLine(string.Format(SuccessfullyImportedDepartment, department.Name, department.Cells.Count));
            }

            context.Departments.AddRange(validDepartments);
            context.SaveChanges();

            return stringBuilder.ToString().TrimEnd();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            StringBuilder stringBuilder = new StringBuilder();

            ImportPrisonerDto[] prisonerDtos = JsonConvert.DeserializeObject<ImportPrisonerDto[]>(jsonString);

            ICollection<Prisoner> validPrisoners = new HashSet<Prisoner>();


            foreach (ImportPrisonerDto prisonerDto in prisonerDtos)
            {

                if (!IsValid(prisonerDto))
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime incarcerationDate;
                bool isIncarcerationDateValid = DateTime.TryParseExact(prisonerDto.IncarcerationDate, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out incarcerationDate);

                if (!isIncarcerationDateValid)
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime releaseDate;
                bool isReleaseDateValid = DateTime.TryParseExact(prisonerDto.ReleaseDate, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out releaseDate);

                if (!isReleaseDateValid)
                {
                    if (prisonerDto.ReleaseDate != null)
                    {
                        stringBuilder.AppendLine(ErrorMessage);
                        continue;
                    }
                }

                Prisoner prisoner = new Prisoner()
                {
                    FullName = prisonerDto.FullName,
                    Nickname = prisonerDto.Nickname,
                    Age = prisonerDto.Age,
                    IncarcerationDate = incarcerationDate,
                    ReleaseDate = releaseDate == null? null: releaseDate,
                    CellId = prisonerDto.CellId
                };

                foreach (ImportMailDto mailDto in prisonerDto.Mails)
                {

                    if (!IsValid(mailDto))
                    {
                        stringBuilder.AppendLine(ErrorMessage);
                        continue;
                    }

                    Mail mail = new Mail()
                    {
                        Description = mailDto.Description,
                        Sender = mailDto.Sender,
                        Address = mailDto.Address
                    };

                    prisoner.Mails.Add(mail);
                }

                validPrisoners.Add(prisoner);
                stringBuilder.AppendLine(string.Format(SuccessfullyImportedPrisoner, prisoner.FullName, prisoner.Age));
            }

            context.Prisoners.AddRange(validPrisoners);
            context.SaveChanges();

            return stringBuilder.ToString().TrimEnd();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            XmlHelper xmlHelper = new XmlHelper();
            StringBuilder stringBuilder = new StringBuilder();

            ImportOfficerDto[] officerDtos = xmlHelper.Deserialize<ImportOfficerDto[]>(xmlString, "Officers");

            ICollection<Officer> validOfficers = new HashSet<Officer>();

            foreach (ImportOfficerDto officerDto in officerDtos)
            {
                if (!IsValid(officerDto))
                {
                    stringBuilder.Append(ErrorMessage);
                    continue;
                }

                object positionTypeObj;
                bool isPositionTypeValid =
                    Enum.TryParse(typeof(Position), officerDto.Position, out positionTypeObj);

                if (!isPositionTypeValid)
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                Position positionType = (Position)positionTypeObj;

                object weaponTypeObj;
                bool isWeaponTypeValid =
                    Enum.TryParse(typeof(Weapon), officerDto.Weapon, out weaponTypeObj);

                if (!isWeaponTypeValid)
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                Weapon weaponTipe = (Weapon)weaponTypeObj;

                Officer officer = new Officer()
                {
                    FullName = officerDto.FullName,
                    Salary = officerDto.Salary,
                    Position = positionType,
                    Weapon = weaponTipe,
                    DepartmentId = officerDto.DepartmentId
                };


                foreach (ImportOfficerPrisonerDto prisonerDto in officerDto.Prisoners)
                {
                    officer.OfficerPrisoners.Add(new OfficerPrisoner()
                    {
                        Officer = officer,
                        PrisonerId = prisonerDto.PrisonerId
                    });
                }

                validOfficers.Add(officer);
                stringBuilder.AppendLine(String.Format(SuccessfullyImportedOfficer, officer.FullName, officer.OfficerPrisoners.Count));
            }

            context.Officers.AddRange(validOfficers);
            context.SaveChanges();

            return stringBuilder.ToString().TrimEnd();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}