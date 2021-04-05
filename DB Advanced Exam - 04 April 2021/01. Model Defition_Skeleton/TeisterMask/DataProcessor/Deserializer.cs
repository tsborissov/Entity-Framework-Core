namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;

    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.Data.Models;
    using TeisterMask.Data.Models.Enums;
    using TeisterMask.DataProcessor.ImportDto;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            var xmlRoot = new XmlRootAttribute("Projects");
            var serializer = new XmlSerializer(typeof(ProjectXmlImportModel[]), xmlRoot);
            var inputXmlReader = new StringReader(xmlString);
            var dtoProjects = (ProjectXmlImportModel[])serializer.Deserialize(inputXmlReader);

            var sb = new StringBuilder();

            var validProjects = new List<Project>();

            foreach (var projectDto in dtoProjects)
            {
                if (!IsValid(projectDto))
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                DateTime.TryParseExact(projectDto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
                        out DateTime currentProjectOpenDate);


                var isProjectDueDateParsed = DateTime.TryParseExact(projectDto.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
                        out DateTime currentProjectDueDate);



                var project = new Project 
                {
                    Name = projectDto.Name,
                    OpenDate = currentProjectOpenDate,
                };

                if (isProjectDueDateParsed)
                {
                    project.DueDate = currentProjectDueDate;
                }

                foreach (var taskDto in projectDto.Tasks)
                {
                    if (!IsValid(taskDto))
                    {
                        sb.AppendLine("Invalid data!");
                        continue;
                    }

                    DateTime.TryParseExact(taskDto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
                            out DateTime currentTaskOpenDate);

                    DateTime.TryParseExact(taskDto.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
                            out DateTime currentTaskDueDate);

                    if (currentTaskOpenDate < currentProjectOpenDate ||
                        (isProjectDueDateParsed && currentTaskDueDate > currentProjectDueDate))
                    {
                        sb.AppendLine("Invalid data!");
                        continue;
                    }

                    var executionType = (ExecutionType)taskDto.ExecutionType;

                    var task = new Task 
                    {
                        Name = taskDto.Name,
                        OpenDate = currentTaskOpenDate,
                        DueDate = currentTaskDueDate,
                        ExecutionType = (ExecutionType)taskDto.ExecutionType,
                        LabelType = (LabelType)taskDto.LabelType,
                    };

                    project.Tasks.Add(task);
                }

                validProjects.Add(project);

                sb.AppendLine($"Successfully imported project - {project.Name} with {project.Tasks.Count} tasks.");
            }

            context.Projects.AddRange(validProjects);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            var data = JsonConvert.DeserializeObject<IEnumerable<EmployeeJsonImportModel>>(jsonString);

            var sb = new StringBuilder();

            var existingTasksIds = context.Tasks.Select(x => x.Id).ToArray();

            foreach (var employeeDto in data)
            {
                if (!IsValid(employeeDto))
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                var employee = new Employee 
                {
                    Username = employeeDto.Username,
                    Email = employeeDto.Email,
                    Phone = employeeDto.Phone,
                };


                foreach (var taskId in employeeDto.Tasks.Distinct())
                {
                    if (!existingTasksIds.Contains(taskId))
                    {
                        sb.AppendLine("Invalid data!");
                        continue;
                    }

                    employee.EmployeesTasks.Add(new EmployeeTask { TaskId = taskId });
                }

                sb.AppendLine($"Successfully imported employee - {employee.Username} with {employee.EmployeesTasks.Count} tasks.");
                
                context.Employees.Add(employee);
                context.SaveChanges();

            }

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