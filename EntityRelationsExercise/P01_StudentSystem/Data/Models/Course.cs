using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace P01_StudentSystem.Data.Models
{
    public class Course
    {
        //o CourseId
        //o Name(up to 80 characters, unicode)
        //o Description(unicode, not required)
        //o StartDate
        //o EndDate
        //o Price

        public Course()
        {
            this.Resources = new HashSet<Resource>();
            this.StudentsEnrolled = new HashSet<StudentCourse>();
            this.HomeworkSubmissions = new HashSet<Homework>();
        }

        public int CourseId { get; set; }

        [Required]
        [MaxLength(80)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public decimal Price { get; set; }

        public virtual ICollection<StudentCourse> StudentsEnrolled { get; set; }

        public virtual ICollection<Resource> Resources { get; set; }

        public virtual ICollection<Homework> HomeworkSubmissions { get; set; }
    }
}
