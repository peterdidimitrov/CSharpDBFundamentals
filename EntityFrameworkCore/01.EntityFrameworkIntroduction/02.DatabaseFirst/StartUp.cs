namespace SoftUni;

using System.Linq;
using System.Text;

using Data;
using Microsoft.EntityFrameworkCore;
using Models;

public class StartUp
{
    static void Main(string[] args)
    {
        SoftUniContext dbContext = new SoftUniContext();

        string result = RemoveTown(dbContext);
        Console.WriteLine(result);
    }

    //3.	Employees Full Information
    public static string GetEmployeesFullInformation(SoftUniContext context)
    {
        StringBuilder sb = new StringBuilder();

        var employees = context.Employees
            .OrderBy(e => e.EmployeeId)
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.MiddleName,
                e.JobTitle,
                e.Salary
            })
            .ToArray();

        foreach (var employee in employees)
        {
            sb.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:f2}");
        }
        return sb.ToString().TrimEnd();
    }

    //4.	Employees with Salary Over 50 000
    public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
    {
        StringBuilder sb = new StringBuilder();

        var employees = context.Employees
            .OrderBy(e => e.FirstName)
            .Where(e => e.Salary > 50000)
            .Select(e => new
            {
                e.FirstName,
                e.Salary
            })
            .ToArray();

        foreach (var employee in employees)
        {
            sb.AppendLine($"{employee.FirstName} - {employee.Salary:f2}");
        }
        return sb.ToString().TrimEnd();
    }

    //5.	Employees from Research and Development
    public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
    {
        StringBuilder sb = new StringBuilder();

        var employees = context.Employees
            .OrderBy(e => e.Salary)
            .ThenByDescending(e => e.FirstName)
            .Where(e => e.Department.Name == "Research and Development")
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                DepartmentName = e.Department.Name,
                e.Salary
            })
            .ToArray();

        foreach (var employee in employees)
        {
            sb.AppendLine($"{employee.FirstName} {employee.LastName} from {employee.DepartmentName} - ${employee.Salary:f2}");
        }
        return sb.ToString().TrimEnd();
    }

    //6.	Adding a New Address and Updating Employee
    public static string AddNewAddressToEmployee(SoftUniContext context)
    {
        Address address = new Address()
        {
            AddressText = "Vitoshka 15",
            TownId = 4
        };

        var employee = context.Employees
            .FirstOrDefault(e => e.LastName == "Nakov");

        employee.Address = address;

        context.SaveChanges();

        StringBuilder sb = new StringBuilder();

        var employees = context.Employees
            .OrderByDescending(e => e.AddressId)
            .Take(10)
            .Select(e => new
            {
                AddressText = e.Address.AddressText
            })
            .ToArray();

        foreach (var entity in employees)
        {
            sb.AppendLine($"{entity.AddressText}");
        }
        return sb.ToString().TrimEnd();
    }

    //07. Employees and Projects
    public static string GetEmployeesInPeriod(SoftUniContext context)
    {
        StringBuilder sb = new StringBuilder();

        var employeesWithProjects = context.Employees
            .Take(10)
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                ManagerFirstName = e.Manager!.FirstName,
                ManagerLastName = e.Manager!.LastName,
                Projects = e.EmployeesProjects
                    .Select(ep => new
                    {
                        ProjectName = ep.Project!.Name,
                        StartDate = ep.Project.StartDate,
                        EndDate = ep.Project.EndDate.HasValue ?
                            ep.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt") :
                            "not finished"
                    })
            })
            .ToArray();

        foreach (var employee in employeesWithProjects)
        {

            sb.AppendLine($"{employee.FirstName} {employee.LastName} - Manager: {employee.ManagerFirstName} {employee.ManagerLastName}");

            foreach (var project in employee.Projects)
            {
                if (project.StartDate.Year >= 2001 & project.StartDate.Year <= 2003)
                {
                    sb.AppendLine($"--{project.ProjectName} - {project.StartDate.ToString("M/d/yyyy h:mm:ss tt")} - {project.EndDate}");
                }
            }

        }
        return sb.ToString().TrimEnd();
    }

    //8.	Addresses by Town
    public static string GetAddressesByTown(SoftUniContext context)
    {
        StringBuilder sb = new StringBuilder();

        var addresses = context.Addresses
            .Select(a => new
            {
                a.AddressText,
                TownName = a.Town!.Name,
                EmployeeCount = context.Employees
                    .Where(e => e.AddressId == a.AddressId).Select(e => new
                    {

                    }).Count()
            })
            .OrderByDescending(a => a.EmployeeCount)
            .ThenBy(a => a.TownName)
            .ThenBy(a => a.AddressText)
            .Take(10)
            .ToArray();

        foreach (var address in addresses)
        {

            sb.AppendLine($"{address.AddressText}, {address.TownName} - {address.EmployeeCount} employees");
        }
        return sb.ToString().TrimEnd();
    }

    //9.	Employee 147
    public static string GetEmployee147(SoftUniContext context)
    {
        StringBuilder sb = new StringBuilder();

        var employee = context.Employees
            .Include(e => e.EmployeesProjects)
            .ThenInclude(ep => ep.Project)
            .Where(e => e.EmployeeId == 147)
            .FirstOrDefault();

        sb.AppendLine($"{employee!.FirstName} {employee.LastName} - {employee.JobTitle}");

        foreach (var employeeProject in employee.EmployeesProjects.OrderBy(p => p.Project!.Name))
        {
            sb.AppendLine(employeeProject.Project!.Name);
        }
        return sb.ToString().TrimEnd();
    }

    //10.	Departments with More Than 5 Employees
    public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
    {
        StringBuilder sb = new StringBuilder();

        var departments = context.Departments
                    .Select(d => new
                    {
                        DepartmentName = d.Name,
                        ManagerName = $"{d.Manager.FirstName} {d.Manager.LastName}",
                        DepartmentId = d.DepartmentId,
                        Employees = context.Employees
                            .Select(e => new
                            {
                                EmployeeFirstName = e.FirstName,
                                EmployeeLastName = e.LastName,
                                DepartmentId = e.DepartmentId,
                                Job = e.JobTitle
                            })
                            .Where(e => e.DepartmentId == d.DepartmentId)
                            .OrderBy(e => e.EmployeeFirstName)
                            .ThenBy(e => e.EmployeeLastName)
                            .ToArray()
                    })
                    .Where(d => d.Employees.Count() > 5)
                    .OrderBy(e => e.Employees.Count())
                    .ThenBy(e => e.DepartmentName)
                    .ToArray();

        foreach (var department in departments)
        {
            sb.AppendLine($"{department.DepartmentName} - {department.ManagerName}");

            foreach (var employee in department.Employees)
            {
                sb.AppendLine($"{employee.EmployeeFirstName} {employee.EmployeeLastName} - {employee.Job}");
            }
        }
        return sb.ToString().TrimEnd();
    }
    //11. Find Latest 10 Projects
    public static string GetLatestProjects(SoftUniContext context)
    {
        StringBuilder sb = new StringBuilder();

        var projects = context.Projects
                    .Select(p => new
                    {
                        p.Name,
                        p.Description,
                        StgartDate = p.StartDate
                    })
                    .OrderByDescending(p => p.StgartDate)
                    .ToArray();

        foreach (var project in projects.Take(10).OrderBy(p => p.Name))
        {
            sb.AppendLine($"{project.Name}");
            sb.AppendLine($"{project.Description}");
            sb.AppendLine($"{project.StgartDate.ToString("M/d/yyyy h:mm:ss tt")}");
        }
        return sb.ToString().TrimEnd();
    }
    //12.	Increase Salaries
    public static string IncreaseSalaries(SoftUniContext context)
    {
        var sb = new StringBuilder();

        decimal increasePercentage = 12;

        var employees = context.Employees
            .Where(e => e.Department.Name == "Engineering" ||
                        e.Department.Name == "Tool Design" ||
                        e.Department.Name == "Marketing" ||
                        e.Department.Name == "Information Services");

        foreach (var employee in employees.OrderBy(e => e.FirstName).ThenBy(e => e.LastName))
        {
            employee.Salary *= (increasePercentage / 100 + 1);
            //employeesToPrint.Add(employee);
            sb.AppendLine($"{employee.FirstName} {employee.LastName} (${employee.Salary:f2})");
        }

        context.SaveChanges();

        return sb.ToString().TrimEnd();
    }
    //13.	Find Employees by First Name Starting with "Sa"
    public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
    {
        var sb = new StringBuilder();

        var employees = context.Employees
            .Where(e => e.FirstName.StartsWith("Sa"))
            .OrderBy(e => e.FirstName)
            .ThenBy(e => e.LastName)
            .Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.JobTitle,
                e.Salary
            })
            .ToArray();

        foreach (var employee in employees)
        {
            sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle} - (${employee.Salary:f2})");
        }

        return sb.ToString().TrimEnd();
    }

    //14.	Delete Project by Id
    public static string DeleteProjectById(SoftUniContext context)
    {
        StringBuilder sb = new StringBuilder();

            var project = context.Projects.Find(2);

            context.EmployeesProjects.Where(ep => ep.ProjectId == project!.ProjectId).ToList().ForEach(ep => context.EmployeesProjects.Remove(ep));

            context.Projects.Remove(project!);

            context.SaveChanges();

            context.Projects.Take(10).Select(p => p.Name).ToList().ForEach(p => sb.AppendLine(p));

        return sb.ToString().TrimEnd();
    }

    //15.	Remove Town
    public static string RemoveTown(SoftUniContext context)
    {
        int count = 0;
        string townName = "Seattle";

        context.Employees
            .Where(e => e.Address!.Town!.Name == townName)
            .ToList()
            .ForEach(e => e.AddressId = null);

        count = context.Addresses
                    .Where(a => a.Town!.Name == townName)
                    .Count();

        context.Addresses
                    .Where(a => a.Town!.Name == townName)
                    .ToList()
                    .ForEach(a => context.Addresses.Remove(a));

        context.Towns
            .Remove(context.Towns
                .SingleOrDefault(t => t.Name == townName));

        context.SaveChanges();

        return $"{count} addresses in Seattle were deleted";
    }
}