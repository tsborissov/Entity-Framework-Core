namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.DataProcessor.ExportDto;
    using System;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            var targetPrisoners = context.Prisoners
                .Where(x => ids.Contains(x.Id))
                .Select(p => new PrisonerByCellExportModel
                {
                    Id = p.Id,
                    Name = p.FullName,
                    CellNumber = p.Cell.CellNumber,
                    Officers = p.PrisonerOfficers
                    .Select(o => new OfficerExportModel
                    {
                        OfficerName = o.Officer.FullName,
                        Department = o.Officer.Department.Name
                    })
                    .OrderBy(x => x.OfficerName)
                    .ToArray(),
                    TotalOfficerSalary = decimal.Parse(p.PrisonerOfficers
                    .Sum(x => x.Officer.Salary)
                    .ToString("F2"))
                })
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToArray();

            var result = JsonConvert.SerializeObject(targetPrisoners, Formatting.Indented);

            return result;
        }


        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            var targetNames = prisonersNames.Split(',').ToArray();

            var targetPrisoners = context.Prisoners
                .Where(x => targetNames.Contains(x.FullName))
                .Select(p => new PrisonerInboxExportModel
                {
                    Id = p.Id,
                    Name = p.FullName,
                    IncarcerationDate = p.IncarcerationDate.ToString("yyyy-MM-dd"),
                    EncryptedMessages = p.Mails.Select(m => new MessageExportModel
                    {
                        Description = new string(m.Description.Reverse().ToArray())
                    })
                    .ToArray()
                })
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToArray();

            var xmlRoot = new XmlRootAttribute("Prisoners");
            var serializer = new XmlSerializer(typeof(PrisonerInboxExportModel[]), xmlRoot);
            var textWriter = new StringWriter();
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            serializer.Serialize(textWriter, targetPrisoners, ns);

            return textWriter.ToString();
        }
    }
}
