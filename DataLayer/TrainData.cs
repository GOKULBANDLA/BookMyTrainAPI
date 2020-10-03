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
        /// <summary>
        /// Returns the Trains based on Search Criteria
        /// </summary>
        /// <returns></returns>
        public async Task<DataTable> FetchTrains(TrainSearch search)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_appSettings.Value.DbConnection))
                {
                    await sqlConnection.OpenAsync();

                    SqlDataAdapter da = new SqlDataAdapter("FetchTrains", sqlConnection);

                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    da.SelectCommand.Parameters.Add("@from", SqlDbType.Int).Value = search.Source;
                    da.SelectCommand.Parameters.Add("@to", SqlDbType.Int).Value = search.Destination;
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
        /// <summary>
        /// Method returns the previous booking details
        /// </summary>
        /// <returns></returns>
        public async Task<List<BookingDetails>> FetchBookings(TrainSearch search)
        {
            List<BookingDetails> lstBookings = new List<BookingDetails>();
            
            try
            {
                DateTime dateOfJourney = DateTime.Parse(search.DateOfJourney.ToString());
                using (SqlConnection sqlConnection = new SqlConnection(_appSettings.Value.DbConnection))
                {
                    await sqlConnection.OpenAsync();
                    string command = "Select trainId,Count(*) as 'count' from Bookings where source=" + search.Source + " and destination=" + search.Destination + " and dateOfJourney = '" + dateOfJourney.ToString("yyyy-MM-dd") + "' group by trainId";
                    SqlCommand cmd = new SqlCommand(command, sqlConnection);
                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        BookingDetails booking = new BookingDetails();
                        booking.TrainId = Convert.ToInt32(rdr["trainId"]);
                        booking.Count = Convert.ToInt32(rdr["count"]);
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
        /// <summary>
        /// Returns the list of stations 
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// Insert new ticket booking record into database 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> BookTrain(TrainBookingDetails bookingDetails)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_appSettings.Value.DbConnection))
                {
                    await sqlConnection.OpenAsync();
                    string command = "Insert Into dbo.Bookings (trainId, source, destination, dateOfJourney, bookingTime) " +
                   "VALUES (@trainId, @source, @destination, @date, @bookingTime) ";
                    SqlCommand cmd = new SqlCommand(command, sqlConnection);
                    cmd.Parameters.Add("@trainId", SqlDbType.Int, 100).Value = bookingDetails.TrainId;
                    cmd.Parameters.Add("@source", SqlDbType.Int, 100).Value = bookingDetails.SourceId;
                    cmd.Parameters.Add("@destination", SqlDbType.Int, 100).Value = bookingDetails.DestinationId;
                    cmd.Parameters.Add("@date", SqlDbType.Date, 100).Value = DateTime.Parse(bookingDetails.DateOfJourney.ToString());
                    cmd.Parameters.Add("@bookingTime", SqlDbType.DateTime, 100).Value = DateTime.Now.ToString();
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
