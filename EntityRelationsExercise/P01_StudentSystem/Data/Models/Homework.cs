using P01_StudentSystem.Data.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P01_StudentSystem.Data.Models
{
    public class Homework
    {
        //o HomeworkId
        //o Content(string, linking to a file, not unicode)
        //o ContentType(enum – can be Application, Pdf or Zip)
        //o SubmissionTime
        //o StudentId
        //o CourseId

        public int HomeworkId { get; set; }

        [Required]
        [Column(TypeName = "varchar(255)")]
        public string Content { get; set; }

        public ContentType ContentType { get; set; }

        public DateTime SubmissionTime { get; set; }

        public int StudentId { get; set; }

        public int CourseId { get; set; }

        public virtual Student Student { get; set; }

        public virtual Course Course { get; set; }
    }
}
