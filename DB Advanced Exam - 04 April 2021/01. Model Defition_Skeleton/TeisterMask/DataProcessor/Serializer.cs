namespace TeisterMask.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.Data.Models.Enums;
    using TeisterMask.DataProcessor.ExportDto;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            var projects = context.Projects.ToArray()
                .Where(x => x.Tasks.Any())
                .Select(p => new ProjectXmlExportModel 
                {
                    TasksCount = p.Tasks.Count,
                    ProjectName = p.Name,
                    HasEndDate = p.DueDate.ToString(),
                    Tasks = p.Tasks.Select(t => new TaskXmlExportModel 
                    {
                        Name = t.Name,
                        Label = t.LabelType.ToString()
                    })
                    .OrderBy(x => x.Name)
                    .ToArray()
                })
                .OrderByDescending(x => x.Tasks.Count())
                .ThenBy(x => x.ProjectName)
                .ToArray();

            //foreach (var project in projects)
            //{
            //    if (project.HasEndDate == "")
            //    {
            //        project.HasEndDate = "No";
            //    }
            //    else
            //    {
            //        project.HasEndDate = "Yes";
            //    }
            //}


            var xmlRootAttribute = new XmlRootAttribute("Projects");
            var serializer = new XmlSerializer(typeof(ProjectXmlExportModel[]), xmlRootAttribute);
            var textWriter = new StringWriter();
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            serializer.Serialize(textWriter, projects, ns);

            return textWriter.ToString();
        }

        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            var employees = context.Employees.ToArray()
                .Where(x => x.EmployeesTasks.Any(t => t.Task.OpenDate >= date))
                .Select(x => new
                {
                    Username = x.Username,
                    Tasks = x.EmployeesTasks
                    .Where(x => x.Task.OpenDate >= date)
                    .OrderByDescending(d => d.Task.DueDate)
                    .ThenBy(x => x.Task.Name)
                    .Select(t => new 
                    {
                        TaskName = t.Task.Name,
                        OpenDate = t.Task.OpenDate.ToString("d", CultureInfo.InvariantCulture),
                        DueDate = t.Task.DueDate.ToString("d", CultureInfo.InvariantCulture),
                        LabelType = t.Task.LabelType.ToString(),
                        ExecutionType = t.Task.ExecutionType.ToString(),
                    })
                    .ToArray()
                })
                .OrderByDescending(x => x.Tasks.Count())
                .ThenBy(x => x.Username)
                .Take(10)
                .ToArray();
                
            return JsonConvert.SerializeObject(employees, Formatting.Indented);
        }
    }
}