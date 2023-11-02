using System.Text;
using Microsoft.EntityFrameworkCore;
using SoftUni.Data;
using SoftUni.Models;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();

            Console.WriteLine(RemoveTown(context));
        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees.Select(x => new
            {
                x.EmployeeId,
                x.FirstName,
                x.LastName,
                x.MiddleName,
                x.JobTitle,
                x.Salary
            }).OrderBy(x => x.EmployeeId).ToList();

            return string.Join(Environment.NewLine, employees.Select(x => $"{x.FirstName} {x.LastName} {x.MiddleName} {x.JobTitle} {x.Salary:F2}"));
        }

        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context.Employees.Where(x => x.Salary > 50000).OrderBy(x => x.FirstName).Select(x => new { x.FirstName, x.Salary });

            return string.Join(Environment.NewLine, employees.Select(x => $"{x.FirstName} - {x.Salary:F2}"));
        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var emp = context.Employees.Where(e => e.Department.Name == "Research and Development")
                .OrderBy(x => x.Salary).ThenByDescending(x => x.FirstName);

            return string.Join(Environment.NewLine, emp.Select(x => $"{x.FirstName} {x.LastName} from {x.Department.Name} - ${x.Salary:F2}"));
        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            var address = new Address() { AddressText = "Vitoshka 15", TownId = 4 };

            context.Employees.FirstOrDefault(x => x.LastName == "Nakov").Address = address;

            context.SaveChanges();

            var employees = context.Employees.OrderByDescending(x => x.AddressId).Select(x => new { x.Address.AddressText }).Take(10);



            return string.Join(Environment.NewLine, employees.Select(x => $"{x.AddressText}"));

        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var dataString = new StringBuilder();

            var employees = context.Employees
                .Take(10)
                .Include(e => e.Manager)
                .Include(e => e.EmployeesProjects)
                .ToArray();

            for (int i = 0; i < 10; i++)
            {
                var e = employees[i];

                dataString.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.Manager?.FirstName} {e.Manager.LastName}");

                if (e.EmployeesProjects.Count > 0)
                {
                    var projects = context.EmployeesProjects
                        .Select(ep => new
                        {
                            ep.EmployeeId,
                            ep.Project.Name,
                            ep.Project.StartDate,
                            ep.Project.EndDate,
                        })
                        .Where(ep => ep.EmployeeId == e.EmployeeId)
                        .ToArray();

                    foreach (var p in projects)
                    {
                        if (p.StartDate.Year >= 2001 && p.StartDate.Year <= 2003)
                        {
                            var enddate = p.EndDate != null ? p.EndDate?.ToString("M/d/yyyy h:mm:ss tt") : "not finished";

                            dataString.AppendLine($"--{p.Name} - {p.StartDate.ToString("M/d/yyyy h:mm:ss tt")} - {enddate}");
                        }
                    }
                }
            }

            return dataString.ToString().Trim();
        }

        public static string GetAddressesByTown(SoftUniContext context)
        {
            var adr = context.Addresses    
                .OrderByDescending(e => e.Employees.Count)
                .ThenBy(e => e.Town.Name).ThenBy(e => e.AddressText)
                .Select(x => new
                {
                    x.AddressText,
                    x.Town.Name,
                    x.Employees.Count

                }).Take(10);

            return string.Join(Environment.NewLine, adr.Select(x => $"{x.AddressText}, {x.Name} - {x.Count} employees"));
        }

        public static string GetEmployee147(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var emp147 = context.Employees.Include(e => e.EmployeesProjects)
                .ThenInclude(x=>x.Project).FirstOrDefault(x => x.EmployeeId == 147);

     

            sb.AppendLine(emp147.FirstName + " " + emp147.LastName + " - " + emp147.JobTitle);

            foreach (var project in emp147.EmployeesProjects.OrderBy(x=>x.Project.Name))
            {
                sb.AppendLine(project.Project.Name);
            }

            return sb.ToString();
        }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context.Departments.Include(x=>x.Employees)
                .Where(x => x.Employees.Count > 5)
                .OrderBy(x=>x.Employees.Count)
                .OrderBy(x=>x.Name);

            StringBuilder sb = new StringBuilder();
            foreach (var dep in departments)
            {
                sb.AppendLine($"{dep.Name} - {dep.Manager.FirstName}  {dep.Manager.LastName}");
                foreach (var emp in dep.Employees.OrderBy(x=>x.FirstName).ThenBy(x=>x.LastName))
                {
                    sb.AppendLine($"{emp.FirstName} {emp.LastName} - {emp.JobTitle}");
                }

            }

            return sb.ToString();
        }

        public static string GetLatestProjects(SoftUniContext context)
        {
            var proj = context.Projects.OrderByDescending(x => x.StartDate).Take(10).OrderBy(x => x.Name).Select(x => new
            {
                x.Name,
                x.Description,
                x.StartDate
            });

    
   
            return string.Join(Environment.NewLine,proj.Select(x=>$"{x.Name}\n{x.Description}\n{x.StartDate.ToString("M/d/yyyy h:mm:ss tt")}"));
        }

        public static string IncreaseSalaries(SoftUniContext context)
        {
            var empl = context.Employees
                .Where(x => x.Department.Name == "Engineering" || x.Department.Name == "Tool Design" || x.Department.Name == "Marketing" || x.Department.Name == "Information Services").OrderBy(x=>x.FirstName).ThenBy(x=>x.LastName);

            StringBuilder sb = new StringBuilder();
            foreach (var item in empl)
            {
                item.Salary += item.Salary * 0.12m;
                sb.AppendLine($"{item.FirstName} {item.LastName} (${item.Salary:F2})");
            }
            context.SaveChanges();
            return sb.ToString();
        }

        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var empl = context.Employees.Where(x => x.FirstName.ToLower().Substring(0, 2) == "sa").OrderBy(x => x.FirstName).ThenBy(x => x.LastName);
            return string.Join(Environment.NewLine, empl.Select(x => $"{x.FirstName} {x.LastName} - {x.JobTitle} - (${x.Salary:F2})"));
        }

        public static string DeleteProjectById(SoftUniContext context)
        {
            var project = context.Projects
                          .Find(2);

            context.EmployeesProjects.RemoveRange(context.EmployeesProjects.Where(ep => ep.Project == project));
            context.Projects.Remove(project);

            context.SaveChanges();

            var result = new StringBuilder();

            var projects = context.Projects
                .Take(10)
                .ToList();

            foreach (var p in projects)
            {
                result.AppendLine(p.Name);
            }

            return result.ToString().Trim();
        }

        public static string RemoveTown(SoftUniContext context)
        {
            var town = context.Towns.Include(t => t.Addresses).FirstOrDefault(x => x.Name == "Seattle");

            var Addresses=context.Addresses.Where(x=>x.TownId==town.TownId);

            foreach (var item in context.Employees.Where(x => Addresses.Any(y=>y.AddressId==x.AddressId)))
            {
                item.AddressId = null;
            }


            context.Addresses.RemoveRange(Addresses);

            context.Remove(town);
            context.SaveChanges();

            return $"{town.Addresses.Count} addresses in Seattle were deleted";
        }
    }
}