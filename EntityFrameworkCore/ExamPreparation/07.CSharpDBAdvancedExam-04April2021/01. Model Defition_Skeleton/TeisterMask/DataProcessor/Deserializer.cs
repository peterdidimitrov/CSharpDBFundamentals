// ReSharper disable InconsistentNaming

namespace TeisterMask.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    using Data;
    using System.Diagnostics.Metrics;
    using System.Text;
    using Trucks.Utilities;
    using TeisterMask.DataProcessor.ImportDto;
    using TeisterMask.Data.Models;
    using System.Globalization;
    using Castle.Core.Internal;
    using TeisterMask.Data.Models.Enums;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            StringBuilder stringBuilder = new StringBuilder();

            XmlHelper xmlHelper = new XmlHelper();

            ImportProjectDto[] projectDtos = xmlHelper.Deserialize<ImportProjectDto[]>(xmlString, "Projects");

            ICollection<Project> validProjects = new HashSet<Project>();

            foreach (ImportProjectDto projectDto in projectDtos)
            {
                if (string.IsNullOrEmpty(projectDto.OpenDate))
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                if (!IsValid(projectDto))
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }


                var parseOpenDate = DateTime.ParseExact(projectDto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                Project project = new Project()
                {
                    Name = projectDto.Name,
                    OpenDate = parseOpenDate,
                };


                if (!string.IsNullOrEmpty(projectDto.DueDate))
                {
                    var parseDueDate = DateTime.ParseExact(projectDto.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    project.DueDate = parseDueDate;
                }

                foreach (ImportTaskDto taskDto in projectDto.Tasks)
                {
                    if (string.IsNullOrEmpty(taskDto.OpenDate) || string.IsNullOrEmpty(taskDto.DueDate))
                    {
                        stringBuilder.AppendLine(ErrorMessage);
                        continue;
                    }

                    var parseTaskOpenDate = DateTime.ParseExact(taskDto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    var parseTaskDueDate = DateTime.ParseExact(taskDto.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    if (!string.IsNullOrEmpty(projectDto.DueDate))
                    {
                        if (!IsValid(taskDto)
                            || parseTaskOpenDate < parseOpenDate
                            || parseTaskDueDate > DateTime.ParseExact(projectDto.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture))
                        {
                            stringBuilder.AppendLine(ErrorMessage);
                            continue;
                        }
                    }
                    else
                    {
                        if (!IsValid(taskDto) || parseTaskOpenDate < parseOpenDate)
                        {
                            stringBuilder.AppendLine(ErrorMessage);
                            continue;
                        }
                    }

                    Task task = new Task()
                    {
                        Name = taskDto.Name,
                        OpenDate = parseTaskOpenDate,
                        DueDate = parseTaskDueDate,
                        ExecutionType = (ExecutionType)taskDto.ExecutionType,
                        LabelType = (LabelType)taskDto.LabelType
                    };

                    project.Tasks.Add(task);
                }

                validProjects.Add(project);
                stringBuilder.AppendLine(string.Format(SuccessfullyImportedProject, project.Name, project.Tasks.Count));
            }

            context.Projects.AddRange(validProjects);
            context.SaveChanges();

            return stringBuilder.ToString().TrimEnd();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            StringBuilder stringBuilder = new StringBuilder();

            ImportEmployeeDto[] employeeDtos = JsonConvert.DeserializeObject<ImportEmployeeDto[]>(jsonString);

            ICollection<Employee> validEmployees = new HashSet<Employee>();

            ICollection<int> existingTaskIds = context.Tasks.Select(x => x.Id).ToArray();
            
            foreach (ImportEmployeeDto employeeDto in employeeDtos)
            {

                if (!IsValid(employeeDto))
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                Employee employee = new Employee()
                {
                    Username = employeeDto.Username,
                    Email = employeeDto.Email,
                    Phone = employeeDto.Phone,
                };

                foreach (int taskId in employeeDto.TaskIds.Distinct())
                {
                    if (!existingTaskIds.Contains(taskId))
                    {
                        stringBuilder.AppendLine(ErrorMessage);
                        continue;
                    }

                    EmployeeTask employeeTask = new EmployeeTask()
                    {
                        Employee = employee,
                        TaskId = taskId
                    };
                    employee.EmployeesTasks.Add(employeeTask);
                }

                validEmployees.Add(employee);
                stringBuilder.AppendLine(string.Format(SuccessfullyImportedEmployee, employee.Username, employee.EmployeesTasks.Count));

            }

            context.Employees.AddRange(validEmployees);
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