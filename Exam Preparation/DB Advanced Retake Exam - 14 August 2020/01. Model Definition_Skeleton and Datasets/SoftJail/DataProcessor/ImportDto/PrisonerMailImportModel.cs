using SoftJail.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SoftJail.DataProcessor.ImportDto
{
    public class PrisonerMailImportModel
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string FullName { get; set; }
        //text with min length 3 and max length 20 (required)

        [Required]
        [RegularExpression(@"The [A-Z]{1}[a-z]+")]
        public string Nickname { get; set; }
        //text starting with "The " and a single word only of letters with an uppercase letter for beginning(example: The Prisoner) (required)

        [Range(18, 65)]
        public int Age { get; set; }
        //integer in the range[18, 65] (required)

        [Required]
        public string IncarcerationDate { get; set; }

        public string ReleaseDate { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal? Bail { get; set; }
        //Bail– decimal(non - negative, minimum value: 0)

        public int? CellId { get; set; }

        public IEnumerable<MailImportModel> Mails { get; set; }
    }

    public class MailImportModel
    {
        [Required]
        public string Description { get; set; }

        [Required]
        public string Sender { get; set; }

        [Required]
        [RegularExpression(@"^([0-9A-z\s]+ str.)$")]
        public string Address { get; set; }
        //text, consisting only of letters, spaces and numbers, which ends with “ str.”
    }
}

//{
//    "FullName": "",
//    "Nickname": "The Wallaby",
//    "Age": 32,
//    "IncarcerationDate": "29/03/1957",
//    "ReleaseDate": "27/03/2006",
//    "Bail": null,
//    "CellId": 5,
//    "Mails": [
//      {
//        "Description": "Invalid FullName",
//        "Sender": "Invalid Sender",
//        "Address": "No Address"
//      },
//      {
//        "Description": "Do not put this in your code",
//        "Sender": "My Ansell",
//        "Address": "ha-ha-ha"
//      }
//    ]
//  },

