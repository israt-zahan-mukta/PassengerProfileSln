using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassengerProfile.ViewModels
{
    public class FacilityViewModel
    {
        public int SeatFacilityId { get; set; }
        public string FacilityName { get; set; }
        public decimal Price { get; set; }
        public int PassengerId { get; set; }
    }
}
