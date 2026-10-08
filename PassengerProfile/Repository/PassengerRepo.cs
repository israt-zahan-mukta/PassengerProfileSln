using PassengerProfile.DAL;
using PassengerProfile.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassengerProfile.Repository
{
    public class PassengerRepo
    {
        PassengerGetWay getway = new PassengerGetWay();

        public DataTable GetAllSeatTypes()
        {
            DataTable dt = getway.GetAllSeatTypes();
            return dt;
        }

        public int DeleteSeatFacilitiesByPassengerId(int pId, int sfId)
        {
            int delete = getway.DeleteSeatFacilitiesByPassengerId(pId, sfId);
            return delete;
        }

        public int DeletePassengerByPassengerId(int pId)
        {
            int delete = getway.DeletePassengerByPassengerId(pId);
            return delete;
        }

        public DataTable GetAllPassengers()
        {
            DataTable dt = getway.GetAllPassengers();
            return dt;
        }

        public DataTable GetFacilitiesByPassengerId(int passengerId)
        {
            DataTable dt = getway.GetFacilitiesByPassengerId(passengerId);
            return dt;
        }

        public int SavePassenger(Passenger passenger)
        {
            int count = getway.SavePassenger(passenger);
            return count;
        }

        public int UpdatePassenger(Passenger passenger)
        {
            int count = getway.UpdatePassenger(passenger);
            return count;
        }

        public DataTable GetPassengerByPassengerId(int pId)
        {
            DataTable dt = getway.GetPassengerByPassengerId(pId);
            return dt;
        }

        public DataTable GetAllFacilities()
        {
            DataTable dt = getway.GetAllFacilities();
            return dt;
        }
    }
}
