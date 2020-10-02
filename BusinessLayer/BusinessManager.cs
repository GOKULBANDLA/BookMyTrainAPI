using DataLayer;
using ModelLayer;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using System.Net;

namespace BusinessLayer
{
    public class BusinessManager : IBusinessManager
    {
        private readonly ITrainData _trainData;
        private readonly IOptions<MySettingsModel> _appSettings;
        public BusinessManager(ITrainData data, IOptions<MySettingsModel> app)
        {
            _trainData = data;
            _appSettings = app;
        }

        public async Task<bool> BookTrain(int source, int destination, int trainId, DateTime dateOfJourney)
        {
            return await _trainData.BookTrain(source,destination,trainId,dateOfJourney);
        }

        public async Task<List<Station>> FetchStations()
        {
            try
            {
                return await _trainData.FetchStations();
            }
            catch (Exception)
            {

                throw;
            }
          
        }
        public async Task<List<TrainDetails>> FetchTrains(int source, int destination, DateTime dateOfJourney)
        {
            try
            {
                DataTable dt = await _trainData.FetchTrains(source, destination);
                List<TrainDetails> trainDetailslist = new List<TrainDetails>();
                trainDetailslist = (from DataRow dr in dt.Rows
                                    select new TrainDetails()
                                    {
                                        Seats = Convert.ToInt32(dr["seats"]),
                                        Arrival = dr["arrival"].ToString(),
                                        Departure = dr["departure"].ToString(),
                                        Destination = dr["destination"].ToString(),
                                        RouteId = Convert.ToInt32(dr["routeId"]),
                                        Source = dr["source"].ToString(),
                                        TrainNumber = dr["trainNumber"].ToString(),
                                        TrainName = dr["trainName"].ToString(),
                                        TrainId = Convert.ToInt32(dr["trainId"]),
                                        IsAvailable = true
                                    }).ToList();
                var bookingList = await _trainData.FetchBookings(source, destination, dateOfJourney);
                if (bookingList.Count > 0)
                {

                }
                return trainDetailslist;
            }
            catch (Exception)
            {

                throw;
            }
          
        }

        public  bool SendEmail(string source, string destination, string trainName, string trainNumber, DateTime dateOfJourney)
        {
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress(_appSettings.Value.Email);
            mail.To.Add(_appSettings.Value.Email);
            mail.Subject = "Test Mail";
            mail.Body = "This is for testing SMTP mail from GMAIL";
            var smtpClient = new SmtpClient(_appSettings.Value.Host)
            {
                Port = int.Parse(_appSettings.Value.SMTPPort),
                Credentials = new NetworkCredential(_appSettings.Value.Email, _appSettings.Value.Password),
                EnableSsl = true,

            };
            string userState = "test message1";
               smtpClient.SendAsync(mail,userState);
            return true;
        }
    }
}
