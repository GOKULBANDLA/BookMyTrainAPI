using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ModelLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DataLayer
{
    public class TrainData : ITrainData
    {
        private readonly IOptions<MySettingsModel> _appSettings;
        public TrainData(IOptions<MySettingsModel> app)
        {
            _appSettings = app;
        }
      
        public async Task<DataTable> FetchTrains(int source,int destination)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_appSettings.Value.DbConnection))
                {
                     await sqlConnection.OpenAsync();

                    SqlDataAdapter da = new SqlDataAdapter("FetchTrains", sqlConnection);

                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    da.SelectCommand.Parameters.Add("@from", SqlDbType.Int).Value = source;
                    da.SelectCommand.Parameters.Add("@to", SqlDbType.Int).Value = destination;
                    DataTable table = new DataTable();
                    da.Fill(table);

                    return table;
                }
            }
            catch (Exception)
            {

                throw;
            }
            

        }
        public async Task<List<BookingDetails>> FetchBookings(int source, int destination,DateTime date)
        {
            List<BookingDetails> lstBookings = new List<BookingDetails>();
                
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_appSettings.Value.DbConnection))
                {
                    await sqlConnection.OpenAsync();
                    string command = "Select * from Bookings where source=" + source + " and destination=" + destination;
                    SqlCommand cmd = new SqlCommand(command, sqlConnection);
                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        BookingDetails booking = new BookingDetails();
                        booking.BookingId = Convert.ToInt32(rdr["bookingId"]);
                        booking.TrainId = Convert.ToInt32(rdr["trainId"]);
                        booking.Source = Convert.ToInt32(rdr["source"]);
                        booking.Destination = Convert.ToInt32(rdr["destination"]);
                        booking.DateOfJourney = rdr["dateOfJourney"].ToString();
                        lstBookings.Add(booking);
                    }
                    sqlConnection.Close();
                }
                return lstBookings;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<List<Station>> FetchStations()
        {
            List<Station> lstStation = new List<Station>();

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_appSettings.Value.DbConnection))
                {
                    await sqlConnection.OpenAsync();
                    string command = "Select * from Stations";
                    SqlCommand cmd = new SqlCommand(command, sqlConnection);
                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        Station station = new Station();
                        station.StationId = Convert.ToInt32(rdr["stationId"]);
                       
                        station.StationName = rdr["stationName"].ToString();
                        lstStation.Add(station);
                    }
                    sqlConnection.Close();
                }
                return lstStation;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> BookTrain(int source, int destination, int trainId, DateTime dateOfJourney)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_appSettings.Value.DbConnection))
                {
                    await sqlConnection.OpenAsync();
                    string command = "Insert Into dbo.Bookings (trainId, source, destination, dateOfJourney) " +
                   "VALUES (@trainId, @source, @destination, @date) ";
                    SqlCommand cmd = new SqlCommand(command, sqlConnection);
                    cmd.Parameters.Add("@trainId", System.Data.SqlDbType.Int, 10).Value = trainId;
                    cmd.Parameters.Add("@source", System.Data.SqlDbType.Int, 10).Value = source;
                    cmd.Parameters.Add("@destination", System.Data.SqlDbType.Int, 10).Value = destination;
                    cmd.Parameters.Add("@date", System.Data.SqlDbType.Date, 100).Value = dateOfJourney;
                   await cmd.ExecuteNonQueryAsync();
                    sqlConnection.Close();
                    return true;
                }
        
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
