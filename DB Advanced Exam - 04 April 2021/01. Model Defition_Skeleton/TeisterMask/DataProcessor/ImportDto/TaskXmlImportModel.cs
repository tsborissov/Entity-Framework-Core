using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace TeisterMask.DataProcessor
{
    [XmlType("Task")]
    public class TaskXmlImportModel
    {
        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        [XmlElement("Name")]
        public string Name { get; set; }

        [Required]
        //[RegularExpression(@"^([0-3][0-9]\/[0-1][0-9]\/[0-9]{4})$")]
        [XmlElement("OpenDate")]
        public string OpenDate { get; set; }

        [Required]
        //[RegularExpression(@"^([0-3][0-9]\/[0-1][0-9]\/[0-9]{4})$")]
        [XmlElement("DueDate")]
        public string DueDate { get; set; }

        [XmlElement("ExecutionType")]
        public int ExecutionType { get; set; }

        [XmlElement("LabelType")]
        public int LabelType { get; set; }
    }
}

//•	Id - integer, Primary Key
//•	Name - text with length [2, 40] (required)
//•	OpenDate - date and time(required)
//•	DueDate - date and time(required)
//•	ExecutionType - enumeration of type ExecutionType, with possible values (ProductBacklog, SprintBacklog, InProgress, Finished) (required)
//•	LabelType - enumeration of type LabelType, with possible values (Priority, CSharpAdvanced, JavaAdvanced, EntityFramework, Hibernate) (required)
//•	ProjectId - integer, foreign key(required)
//•	Project - Project
//•	EmployeesTasks - collection of type EmployeeTask



//< Projects >
//  < Project >
//    < Name > S </ Name >
//    < OpenDate > 25 / 01 / 2018 </ OpenDate >
//    < DueDate > 16 / 08 / 2019 </ DueDate >
//    < Tasks >
//      < Task >
//        < Name > Australian </ Name >
//        < OpenDate > 19 / 08 / 2018 </ OpenDate >
//        < DueDate > 13 / 07 / 2019 </ DueDate >
//        < ExecutionType > 2 </ ExecutionType >
//        < LabelType > 0 </ LabelType >
//      </ Task >
//      < Task >
//        < Name > Upland Boneset </ Name >

//           < OpenDate > 24 / 10 / 2018 </ OpenDate >

//           < DueDate > 11 / 06 / 2019 </ DueDate >

//           < ExecutionType > 2 </ ExecutionType >

//           < LabelType > 3 </ LabelType >

//         </ Task >

//       </ Tasks >

//     </ Project >
