using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace TeisterMask.DataProcessor.ExportDto
{
    [XmlType("Project")]
    public class ProjectXmlExportModel
    {
        private string hasEndDate;
        
        [XmlAttribute("TasksCount")]
        public int TasksCount { get; set; }

        [XmlElement("ProjectName")]
        public string ProjectName { get; set; }

        [XmlElement("HasEndDate")]
        public string HasEndDate 
        { 
            get
            {
                if (hasEndDate == "")
                {
                    return "No";
                }
                else
                {
                    return "Yes";
                }
            }
            set
            {
                hasEndDate = value;
            }
        }

        [XmlArray("Tasks")]
        public TaskXmlExportModel[] Tasks { get; set; }
    }
}
