using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SoftJail.DataProcessor.ImportDto
{
    public class DepartmentsCellsImportModel
    {
        [Required]
        [StringLength(25, MinimumLength = 3)]
        public string Name { get; set; }
        //text with min length 3 and max length 25 (required)

        public List<CellImportModel> Cells { get; set; }
    }

    public class CellImportModel
    {
        [Range(1, 1000)]
        public int CellNumber { get; set; }
        //CellNumber – integer in the range [1, 1000] (required)

        public bool HasWindow { get; set; }
    }
}

//{
//    "Name": "",
//    "Cells": [
//      {
//        "CellNumber": 101,
//        "HasWindow": true
//      },
//      {
//        "CellNumber": 102,
//        "HasWindow": false
//      }
//    ]
//  },
