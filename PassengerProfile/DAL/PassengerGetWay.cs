using PassengerProfile.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassengerProfile.DAL
{
    public class PassengerGetWay
    {
        string conStr = ConfigurationManager.ConnectionStrings["db"].ConnectionString;

        public DataTable GetAllSeatTypes()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                string sqlQuery = "SELECT * FROM SeatType";
                SqlCommand cmd = new SqlCommand(sqlQuery, con);
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public DataTable GetAllPassengers()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                string sqlQuery = "SELECT p.ImagePath,p.FisrtName,p.LastName,FORMAT(p.BookingDate,'dd-MMM-yyyy') AS BookingDate,p.MobileNo,p.BasicPrise,st.SeatTypeName, CASE WHEN p.IsTransit='true' THEN 'Tanrsit' ELSE 'No Transit' END AS TransitStatus,st.SeatTypeId,p.PassengerId FROM Passenger AS p JOIN SeatType AS st ON p.SeatTypeId=st.SeatTypeId ORDER BY PassengerId DESC";
                SqlCommand cmd = new SqlCommand(sqlQuery, con);
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public DataTable GetFacilitiesByPassengerId(int passengerId)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                string sqlQuery = "SELECT * FROM SeatFacility WHERE PassengerId ='" + passengerId + "'";
                SqlCommand cmd = new SqlCommand(sqlQuery, con);
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }



        public int DeletePassengerByPassengerId(int pId)
        {
            int count = 0;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand deleteCmd = new SqlCommand("DELETE FROM Passenger WHERE PassengerId=@PassengerId", con, tran))
                        {
                            deleteCmd.CommandType = CommandType.Text;
                            deleteCmd.Parameters.Add("@PassengerId", SqlDbType.Int).Value = pId;
                            count = deleteCmd.ExecuteNonQuery();
                        }
                        tran.Commit();
                        return count;
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                        return 0;
                    }
                }
            }

        }

        public int DeleteSeatFacilitiesByPassengerId(int pId, int sfId)
        {
            int count = 0;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand deleteCmd = new SqlCommand("DELETE FROM SeatFacility WHERE PassengerId=@PassengerId AND SeatFacilityId=@SeatFacilityId", con, tran))
                        {
                            deleteCmd.CommandType = CommandType.Text;
                            deleteCmd.Parameters.Add("@PassengerId", SqlDbType.Int).Value = pId;
                            deleteCmd.Parameters.Add("@SeatFacilityId", SqlDbType.Int).Value = sfId;
                            count = deleteCmd.ExecuteNonQuery();
                        }
                        tran.Commit();
                        return count;
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                        return 0;
                    }
                }
            }
        }

        public int SavePassenger(Passenger passenger)
        {
            int PassengerId = 0;
            int facilityInsertId = 0;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand("INSERT INTO PASSENGER(FisrtName,LastName,BookingDate,IsTransit,BasicPrise,MobileNo,SeatTypeId,ImagePath) VALUES (@FisrtName,@LastName,@BookingDate,@IsTransit,@BasicPrise,@MobileNo,@SeatTypeId,@ImagePath) SELECT SCOPE_IDENTITY();", con, tran);
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("@FisrtName", SqlDbType.VarChar).Value = passenger.FisrtName;
                        cmd.Parameters.Add("@LastName", SqlDbType.VarChar).Value = passenger.LastName;
                        cmd.Parameters.Add("@BookingDate", SqlDbType.DateTime).Value = passenger.BookingDate;
                        cmd.Parameters.Add("@IsTransit", SqlDbType.Bit).Value = passenger.IsTransit;
                        cmd.Parameters.Add("@BasicPrise", SqlDbType.Decimal).Value = passenger.BasicPrise;
                        cmd.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = passenger.MobileNo;
                        cmd.Parameters.Add("@SeatTypeId", SqlDbType.Int).Value = passenger.SeatTypeId;
                        cmd.Parameters.Add("@ImagePath", SqlDbType.VarChar).Value = passenger.ImagePath;
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                            PassengerId = Convert.ToInt32(result);
                        foreach (SeatFacility facility in passenger.Facilities)
                        {
                            SqlCommand cmmd = new SqlCommand("INSERT INTO SeatFacility(FacilityName,Price,PassengerId) VALUES (@FacilityName,@Price,@PassengerId)", con, tran);
                            cmmd.CommandType = CommandType.Text;
                            cmmd.Parameters.Add("@FacilityName", SqlDbType.VarChar).Value = facility.FacilityName;
                            cmmd.Parameters.Add("@Price", SqlDbType.Decimal).Value = facility.Price;
                            cmmd.Parameters.Add("@PassengerId", SqlDbType.Int).Value = PassengerId;
                            facilityInsertId += cmmd.ExecuteNonQuery();
                        }
                        tran.Commit();
                        return PassengerId;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine(ex.Message);
                        throw;
                    }
                }
            }
        }

        public DataTable GetPassengerByPassengerId(int pId)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT p.ImagePath,p.FisrtName,p.LastName,FORMAT(p.BookingDate,'dd-MMM-yyyy') AS BookingDate,p.MobileNo,p.BasicPrise,p.IsTransit,st.SeatTypeName, CASE WHEN p.IsTransit='true' THEN 'Transit' ELSE 'No Transit' END AS TransitStatus,st.SeatTypeName,p.SeatTypeId,p.PassengerId FROM Passenger AS p JOIN SeatType AS st ON p.SeatTypeId=st.SeatTypeId WHERE p.PassengerId='" + pId + "'";
                con.Open();
                var rdr = cmd.ExecuteReader();
                dt.Load(rdr, LoadOption.Upsert);
            }
            return dt;
        }


        public int UpdatePassenger(Passenger passenger)
        {
            int count = 0;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("UPDATE Passenger SET FisrtName=@FisrtName, LastName=@LastName, MobileNo=@MobileNo, BookingDate=@BookingDate, SeatTypeId=@SeatTypeId, BasicPrise= @BasicPrise,ImagePath=@ImagePath,IsTransit=@IsTransit WHERE PassengerId=@PassengerId", con, tran))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.Add("@PassengerId", SqlDbType.Int).Value = passenger.PassengerId;
                            cmd.Parameters.Add("@FisrtName", SqlDbType.VarChar).Value = passenger.FisrtName;
                            cmd.Parameters.Add("@LastName", SqlDbType.VarChar).Value = passenger.LastName;
                            cmd.Parameters.Add("@BookingDate", SqlDbType.DateTime).Value = passenger.BookingDate;
                            cmd.Parameters.Add("@IsTransit", SqlDbType.Bit).Value = passenger.IsTransit;
                            cmd.Parameters.Add("@BasicPrise", SqlDbType.Decimal).Value = passenger.BasicPrise;
                            cmd.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = passenger.MobileNo;
                            cmd.Parameters.Add("@SeatTypeId", SqlDbType.Int).Value = passenger.SeatTypeId;
                            cmd.Parameters.Add("@ImagePath", SqlDbType.VarChar).Value = passenger.ImagePath;
                            count = cmd.ExecuteNonQuery();

                            if (count > 0)
                            {
                                using (SqlCommand dcmd = new SqlCommand("DELETE FROM SeatFacility WHERE PassengerId=@PassengerId", con, tran))
                                {
                                    dcmd.CommandType = CommandType.Text;
                                    dcmd.Parameters.Add("@PassengerId", SqlDbType.Int).Value = passenger.PassengerId;
                                    dcmd.ExecuteNonQuery();
                                }
                            }
                            foreach (SeatFacility facility in passenger.Facilities)
                            {
                                using (SqlCommand mcmd = new SqlCommand("INSERT INTO SeatFacility (PassengerId, FacilityName, Price) VALUES (@PassengerId, @FacilityName, @Price)", con, tran))
                                {
                                    mcmd.CommandType = CommandType.Text;
                                    mcmd.Parameters.Add("@PassengerId", SqlDbType.Int).Value = passenger.PassengerId;
                                    mcmd.Parameters.Add("@FacilityName", SqlDbType.VarChar).Value = facility.FacilityName;
                                    mcmd.Parameters.Add("@Price", SqlDbType.Decimal).Value = facility.Price;
                                    mcmd.ExecuteNonQuery();
                                }
                            }

                        }
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {

                        tran.Rollback();
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return count;
        }

        public DataTable GetAllFacilities()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                string sqlQuery = "SELECT * FROM SeatFacility";
                SqlCommand cmd = new SqlCommand(sqlQuery, con);
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }
    }      
}
