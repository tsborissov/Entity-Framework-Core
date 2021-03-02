using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace P01_HospitalDatabase.Data.Models
{
    public class Doctor
    {
        //name and specialty should be up to 100 characters long, unicode

        public Doctor()
        {
            this.Visitations = new HashSet<Visitation>();
        }

        public int DoctorId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Specialty { get; set; }

        public ICollection<Visitation> Visitations { get; set; }
    }
}
