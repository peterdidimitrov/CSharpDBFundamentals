namespace TeisterMask.DataProcessor
{
    using Data;
    using Microsoft.VisualBasic;
    using Newtonsoft.Json;
    using System.Globalization;
    using TeisterMask.Data.Models.Enums;
    using TeisterMask.DataProcessor.ExportDto;
    using Trucks.Utilities;

    public class Serializer
    {
        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            var employees = context.Employees
            .Where(e => e.EmployeesTasks.Any(et => et.Task.OpenDate >= date))
            .ToArray()
            .Select(e => new
            {
                Username = e.Username,
                Tasks = e.EmployeesTasks
                .Where(et => et.Task.OpenDate >= date)
                .ToArray()
                .OrderByDescending(et => et.Task.DueDate)
                .ThenBy(et => et.Task.Name)
                .Select(et => new
                {
                    TaskName = et.Task.Name,
                    OpenDate = et.Task.OpenDate.ToString("d", CultureInfo.InvariantCulture),
                    DueDate = et.Task.DueDate.ToString("d", CultureInfo.InvariantCulture),
                    LabelType = et.Task.LabelType.ToString(),
                    ExecutionType = et.Task.ExecutionType.ToString(),
                })
                .ToArray()
            })
            .OrderByDescending(e => e.Tasks.Length)
            .ThenBy(e => e.Username)
            .Take(10)
            .ToArray();

            return JsonConvert.SerializeObject(employees, Formatting.Indented);
        }
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            ExportProjectTaskDto[] projects = context.Projects
           .Where(p => p.Tasks.Any())
           .Select(p => new ExportProjectTaskDto
           {
               ProjectName = p.Name,
               TasksCount = p.Tasks.Count(),
               HasEndDate = string.IsNullOrEmpty(p.DueDate.ToString())? "No": "Yes",
               Tasks = p.Tasks
               .Select(t => new ExportTaskDto
               {
                   Name = t.Name,
                   Label = t.LabelType.ToString(),
               })
               .OrderBy(t => t.Name)
               .ToArray()
           })
           .OrderByDescending(p => p.TasksCount)
           .ThenBy(p => p.ProjectName)
           .ToArray();

            XmlHelper xmlHelper = new XmlHelper();

            return xmlHelper.Serialize(projects, "Projects");
        }

    }
}