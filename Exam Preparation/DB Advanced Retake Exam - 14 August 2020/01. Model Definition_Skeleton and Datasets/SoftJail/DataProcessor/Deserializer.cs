namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.Json;
    using System.Xml.Serialization;

    public class Deserializer
    {
        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            var departments = new List<Department>();

            var sb = new StringBuilder();

            var departmentsCellsDto = JsonConvert.DeserializeObject<IEnumerable<DepartmentsCellsImportModel>>(jsonString);

            foreach (var departmentDto in departmentsCellsDto)
            {
                //•	If a department is invalid, do not import its cells.
                //•	If a Department doesn’t have any Cells, he is invalid.
                //•	If one Cell has invalid CellNumber, don’t import the Department.


                if (!IsValid(departmentDto) ||
                    !departmentDto.Cells.All(IsValid) ||
                    !departmentDto.Cells.Any())
                {
                    sb.AppendLine("Invalid Data");

                    continue;
                }

                var department = new Department
                {
                    Name = departmentDto.Name,
                    Cells = departmentDto.Cells.Select(x => new Cell
                    {
                        CellNumber = x.CellNumber,
                        HasWindow = x.HasWindow
                    })
                    .ToList()
                };

                departments.Add(department);

                sb.AppendLine($"Imported {department.Name} with {department.Cells.Count} cells");
            }

            context.Departments.AddRange(departments);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            var prisoners = new List<Prisoner>();

            var sb = new StringBuilder();

            var prisonersMailsDto = JsonConvert.DeserializeObject<ICollection<PrisonerMailImportModel>>(jsonString);

            foreach (var currentPrisonerDto in prisonersMailsDto)
            {
                if (!IsValid(currentPrisonerDto) ||
                    !currentPrisonerDto.Mails.All(IsValid))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var incarcerationDate = DateTime.ParseExact(currentPrisonerDto.IncarcerationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                bool isValidReleaseDate = DateTime
                    .TryParseExact(currentPrisonerDto.ReleaseDate,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime releaseDate);

                var prisoner = new Prisoner
                {
                    FullName = currentPrisonerDto.FullName,
                    Nickname = currentPrisonerDto.Nickname,
                    Age = currentPrisonerDto.Age,
                    IncarcerationDate = incarcerationDate,
                    ReleaseDate = isValidReleaseDate ? (DateTime?)releaseDate : null,
                    Bail = currentPrisonerDto.Bail,
                    CellId = currentPrisonerDto.CellId,
                    Mails = currentPrisonerDto.Mails.Select(x => new Mail
                    {
                        Description = x.Description,
                        Sender = x.Sender,
                        Address = x.Address
                    })
                    .ToList()
                };

                prisoners.Add(prisoner);

                sb.AppendLine($"Imported {prisoner.FullName} {prisoner.Age} years old");
            }

            context.Prisoners.AddRange(prisoners);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            var xmlRoot = new XmlRootAttribute("Officers");
            var serializer = new XmlSerializer(typeof(OfficerPrisonerImportModel[]), xmlRoot);
            var inputXmlReader = new StringReader(xmlString);
            var dtoOfficersPrisoners = (OfficerPrisonerImportModel[])serializer.Deserialize(inputXmlReader);

            var officers = new List<Officer>();

            var sb = new StringBuilder();

            foreach (var currentOfficer in dtoOfficersPrisoners)
            {
                if (!IsValid(currentOfficer))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                Enum.TryParse(currentOfficer.Position, out Position currentPosition);
                Enum.TryParse(currentOfficer.Weapon, out Weapon currentWeapon);

                var officer = new Officer
                {
                    FullName = currentOfficer.Name,
                    Salary = currentOfficer.Money,
                    Position = currentPosition,
                    Weapon = currentWeapon,
                    DepartmentId = currentOfficer.DepartmentId,
                    OfficerPrisoners = currentOfficer.Prisoners.Select(x => new OfficerPrisoner 
                    {
                        PrisonerId = x.Id
                    })
                    .ToList()
                };

                officers.Add(officer);

                sb.AppendLine($"Imported {officer.FullName} ({officer.OfficerPrisoners.Count} prisoners)");
            }

            context.Officers.AddRange(officers);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
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