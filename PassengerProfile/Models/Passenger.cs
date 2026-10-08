using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassengerProfile.Models
{
    public class Passenger
    {
        public int PassengerId { get; set; }
        public string FisrtName { get; set; }
        public string LastName { get; set; }
        public DateTime BookingDate { get; set; }
        public bool IsTransit { get; set; }
        public decimal BasicPrise { get; set; }
        public int SeatTypeId { get; set; }
        public string MobileNo { get; set; }
        public string ImagePath { get; set; }

        public List<SeatFacility> Facilities { get; set; } = new List<SeatFacility>();

    }
}
