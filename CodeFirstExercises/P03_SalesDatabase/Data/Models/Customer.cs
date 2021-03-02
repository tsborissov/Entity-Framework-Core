using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P03_SalesDatabase.Data.Models
{
    public class Customer
    {
        //	CustomerId
        //	Name(up to 100 characters, unicode)
        //	Email(up to 80 characters, not unicode)
        //	CreditCardNumber(string)
        //	Sales

        public Customer()
        {
            this.Sales = new HashSet<Sale>();
        }

        public int CustomerId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [Column(TypeName = "varchar(80)")]
        public string Email { get; set; }

        public string CreditCardNumber { get; set; }

        public ICollection<Sale> Sales { get; set; }
    }
}
