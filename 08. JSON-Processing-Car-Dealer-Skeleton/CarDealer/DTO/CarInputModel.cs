using System;
using System.Collections.Generic;
using System.Text;

namespace CarDealer.DTO
{
    public class CarInputModel
    {
        public CarInputModel()
        {
            this.PartsIds = new HashSet<int>();
        }
        
        public string Make { get; set; }

        public string Model { get; set; }

        public long TravelledDistance { get; set; }

        public ICollection<int> PartsIds { get; set; }
    }
}
