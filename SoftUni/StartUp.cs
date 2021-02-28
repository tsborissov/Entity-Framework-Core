using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Collections.Generic;

namespace SoftUni
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new SoftUniContext();

            using (context)
            {
                var result = RemoveTown(context);

                Console.WriteLine(result);
            }
        }

        public static string RemoveTown(SoftUniContext context)
        {
            var targetTownName = "Seattle";

            var targetEmployees = context.Employees
                .Where(x => x.Address.Town.Name == targetTownName)
                .ToList();
            
            var targetAddresses = context.Addresses
                .Where(t => t.Town.Name == targetTownName)
                .ToList();

            var addressCount = targetAddresses.Count();

            var targetTown = context.Towns
                .FirstOrDefault(x => x.Name == targetTownName);

            foreach (var employee in targetEmployees)
            {
                employee.AddressId = null;
            }

            context.Addresses.RemoveRange(targetAddresses);
            context.Towns.Remove(targetTown);

            context.SaveChanges();

            return $"{addressCount} addresses in {targetTownName} were deleted";
        }

        public static string DeleteProjectById(SoftUniContext context)
        {
            var targetProjectId = 2;

            
            var targetProject = context.Projects
                .Include(e => e.EmployeesProjects)
                .FirstOrDefault(x => x.ProjectId == targetProjectId);

            context.EmployeesProjects.RemoveRange(targetProject.EmployeesProjects);
            context.Projects.Remove(targetProject);

            context.SaveChanges();

            var projects = context.Projects
                .Select(x => new
                {
                    x.Name
                })
                .Take(10)
                .ToList();

            var sb = new StringBuilder();

            foreach (var project in projects)
            {
                sb.AppendLine($"{project.Name}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(x => x.FirstName.StartsWith("Sa"))
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    x.JobTitle,
                    x.Salary
                })
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToList();

            var sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle} - (${(employee.Salary):F2})");
            }


            return sb.ToString().TrimEnd();
        }

        public static string IncreaseSalaries(SoftUniContext context)
        {
            //Engineering, Tool Design, Marketing or Information Services department by 12%

            var targetDepartmentNames = new List<string>()
            {
                "Engineering",
                "Tool Design",
                "Marketing",
                "Information Services"
            };

            var targetEmployees = context.Employees
                .Where(x => targetDepartmentNames.Contains(x.Department.Name))
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToList();

            foreach (var employee in targetEmployees)
            {
                employee.Salary *= 1.12M;
            }

            context.SaveChanges();

            var sb = new StringBuilder();

            foreach (var employee in targetEmployees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} (${(employee.Salary):F2})");
            }


            return sb.ToString().TrimEnd();
        }

        public static string GetLatestProjects(SoftUniContext context)
        {
            var projects = context.Projects
                .OrderByDescending(x => x.StartDate)
                .Select(x => new
                {
                    x.Name,
                    x.Description,
                    x.StartDate
                })
                .Take(10)
                .ToList()
                .OrderBy(x => x.Name);

            var sb = new StringBuilder();

            foreach (var project in projects)
            {
                sb.AppendLine($"{project.Name}");
                sb.AppendLine($"{project.Description}");
                sb.AppendLine($"{project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context.Departments
                .Where(x => x.Employees.Count > 5)
                .OrderBy(x => x.Employees.Count)
                .ThenBy(x => x.Name)
                .Select(x => new
                {
                    DepartmentName = x.Name,
                    ManagerFirstName = x.Manager.FirstName,
                    ManagerLastName = x.Manager.LastName,
                    Employees = x.Employees
                    .OrderBy(x => x.FirstName)
                    .ThenBy(x => x.LastName)
                    .Select(x => new
                    {
                        EmployeeFirstName = x.FirstName,
                        EmployeeLastName = x.LastName,
                        EmployeeJobTitle = x.JobTitle
                    })
                    .ToList()
                })
                .ToList();

            var sb = new StringBuilder();

            foreach (var department in departments)
            {
                sb.AppendLine($"{department.DepartmentName} - {department.ManagerFirstName} {department.ManagerLastName}");

                foreach (var employee in department.Employees)
                {
                    sb.AppendLine($"{employee.EmployeeFirstName} {employee.EmployeeLastName} - {employee.EmployeeJobTitle}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployee147(SoftUniContext context)
        {
            var targetEmployeeId = 147;

            var targetEmployee = context.Employees
                .Select(x => new Employee
                {
                    EmployeeId = x.EmployeeId,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    JobTitle = x.JobTitle,
                    EmployeesProjects = x.EmployeesProjects
                    .Select(p => new EmployeeProject
                    {
                        Project = p.Project
                    })
                    .OrderBy(p => p.Project.Name)
                    .ToArray()
                })
                .FirstOrDefault(x => x.EmployeeId == targetEmployeeId);

            var sb = new StringBuilder();

            
            sb.AppendLine($"{targetEmployee.FirstName} {targetEmployee.LastName} - {targetEmployee.JobTitle}");

            foreach (var project in targetEmployee.EmployeesProjects)
            {
                sb.AppendLine($"{project.Project.Name}");
            }


            return sb.ToString().TrimEnd();
        }

        public static string GetEmployee147ver2(SoftUniContext context)
        {
            var targetEmployeeId = 147;

            var targetEmployee = context.Employees
                .Select(x => new
                {
                    x.EmployeeId,
                    x.FirstName,
                    x.LastName,
                    x.JobTitle,
                    EmployeesProjects = x.EmployeesProjects
                    .OrderBy(p => p.Project.Name)
                    .Select(p => new
                    {
                        Project = p.Project
                    })
                    .ToArray()
                })
                .FirstOrDefault(x => x.EmployeeId == targetEmployeeId);

            var sb = new StringBuilder();


            sb.AppendLine($"{targetEmployee.FirstName} {targetEmployee.LastName} - {targetEmployee.JobTitle}");

            foreach (var project in targetEmployee.EmployeesProjects)
            {
                sb.AppendLine($"{project.Project.Name}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployee147ver3(SoftUniContext context)
        {
            var targetEmployeeId = 147;

            var targetEmployee = context.Employees
                .Include(x => x.EmployeesProjects)
                .ThenInclude(x => x.Project)
                .ToList()
                .Select(x => new
                {
                    x.EmployeeId,
                    x.FirstName,
                    x.LastName,
                    x.JobTitle,
                    EmployeesProjects = x.EmployeesProjects
                    .OrderBy(p => p.Project.Name)
                    .Select(p => new
                    {
                        Project = p.Project
                    })
                    .ToList()
                })
                .FirstOrDefault(x => x.EmployeeId == targetEmployeeId);

            var sb = new StringBuilder();


            sb.AppendLine($"{targetEmployee.FirstName} {targetEmployee.LastName} - {targetEmployee.JobTitle}");

            foreach (var project in targetEmployee.EmployeesProjects)
            {
                sb.AppendLine($"{project.Project.Name}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetAddressesByTown(SoftUniContext context)
        {
            var adresses = context.Addresses
                .Select(x => new
                { 
                    x.AddressText,
                    TownName = x.Town.Name,
                    employeesCount = x.Employees.Count
                })
                .OrderByDescending(x => x.employeesCount)
                .ThenBy(x => x.TownName)
                .ThenBy(x => x.AddressText)
                .Take(10)
                .ToArray();


            var sb = new StringBuilder();

            foreach (var address in adresses)
            {
                sb.AppendLine($"{address.AddressText}, {address.TownName} - {address.employeesCount} employees");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var periodStart = 2001;
            var periodEnd = 2003;

            var employees = context.Employees
                .Include(x => x.EmployeesProjects)
                .ThenInclude(x => x.Project)
                .Where(x => x.EmployeesProjects.Any(x => x.Project.StartDate.Year >= periodStart && x.Project.StartDate.Year <= periodEnd))
                .Select(x => new
                {
                    EmployeeFirstName = x.FirstName,
                    EmployeeLastName = x.LastName,
                    ManagerFirstName = x.Manager.FirstName,
                    ManagerLastName = x.Manager.LastName,
                    Projects = x.EmployeesProjects.Select(p => new 
                    {
                        ProjectName =p.Project.Name,
                        ProjectStartDate = p.Project.StartDate,
                        ProjectEndDate = p.Project.EndDate
                    })
                })
                .Take(10)
                .ToArray();

            var sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.EmployeeFirstName} {employee.EmployeeLastName} - Manager: {employee.ManagerFirstName} {employee.ManagerLastName}");

                foreach (var project in employee.Projects)
                {
                    var endDate = project.ProjectEndDate?.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture) ?? "not finished";

                    sb.AppendLine($"--{project.ProjectName} - {project.ProjectStartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)} - {endDate}");
                }
            }

            return sb.ToString().TrimEnd();

        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            var newAddressText = "Vitoshka 15";
            var targetTownId = 4;
            var targetLastName = "Nakov";

            //var address = new Address()
            //{
            //    AddressText = newAddressText,
            //    TownId = targetTownId
            //};

            //context.Addresses.Add(address);

            //context.SaveChanges();

            //var newAddressId = address.AddressId;

            var targetEmployee = context.Employees
                .FirstOrDefault(x => x.LastName == targetLastName);

            targetEmployee.Address = new Address()
            {
                AddressText = newAddressText,
                TownId = targetTownId
            };

            context.SaveChanges();

            var employees = context.Employees
                .Select(x => new { x.AddressId, x.Address.AddressText })
                .OrderByDescending(x => x.AddressId)
                .Take(10)
                .ToArray();

            var sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.AddressText}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees
                .Select(x => new 
                { 
                    x.EmployeeId, 
                    x.FirstName, 
                    x.LastName, 
                    x.MiddleName, 
                    x.JobTitle, 
                    x.Salary 
                })
                .OrderBy(x => x.EmployeeId)
                .ToArray();

            var result = new StringBuilder();

            foreach (var employee in employees)
            {
                result.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:F2}");
            }

            return result.ToString().TrimEnd();
        }

        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employeesSalaries = context.Employees
                .Select(x => new { x.FirstName, x.Salary })
                .Where(x => x.Salary > 50000)
                .OrderBy(x => x.FirstName)
                .ToArray();

            var sb = new StringBuilder();

            foreach (var employee in employeesSalaries)
            {
                sb.AppendLine($"{employee.FirstName} - {employee.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var targetDepartment = "Research and Development";

            var employees = context.Employees
                .Where(x => x.Department.Name == targetDepartment)
                .Select(x => new 
                {
                    x.FirstName,
                    x.LastName, 
                    DepartmentName = x.Department.Name, 
                    x.Salary })
                .OrderBy(x => x.Salary)
                .ThenByDescending(x => x.FirstName)
                .ToArray();

            var sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} from {employee.DepartmentName} - ${employee.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
