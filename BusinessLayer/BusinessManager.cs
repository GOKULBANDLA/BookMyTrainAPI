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
using Microsoft.Extensions.Logging;

namespace BusinessLayer
{
    public class BusinessManager : IBusinessManager
    {
        private readonly ITrainData _trainData;
        private readonly IOptions<MySettingsModel> _appSettings;
        private readonly ILogger<BusinessManager> _logger;
        public BusinessManager(ITrainData data, IOptions<MySettingsModel> app, ILogger<BusinessManager> logger)
        {
            _trainData = data;
            _appSettings = app;
            _logger = logger;
        }

        public async Task<bool> BookTrain(TrainBookingDetails bookingDetails)
        {
            _logger.LogInformation("Accessed BookTrain Method in BusinessManager");
            try
            {
                return await _trainData.BookTrain(bookingDetails);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<List<Station>> FetchStations()
        {
            _logger.LogInformation("Accessed FetchStations Method in BusinessManager");
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
            _logger.LogInformation("Accessed FetchTrains Method in BusinessManager");
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
                    foreach (var trains in trainDetailslist)
                    {
                        foreach (var bookings in bookingList)
                        {
                            if (bookings.TrainId == trains.TrainId)
                            {
                                int seats = bookings.Count;
                                trains.Seats = trains.Seats - seats;
                                if (trains.Seats <= 0)
                                {
                                    trains.IsAvailable = false;
                                }
                            }
                        }
                    }
                }
                return trainDetailslist;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public bool SendEmail(TrainBookingDetails bookingDetails)
        {
            _logger.LogInformation("Accessed SendEmail Method in BusinessManager");
            MailMessage mail = new MailMessage();
            SmtpClient smtp = new SmtpClient();
            try
            {
                mail.From = new MailAddress(_appSettings.Value.Email);
                mail.To.Add(new MailAddress("gagangokul1997@gmail.com"));
                mail.Subject = "Ticket Booking Confirmation";
                mail.IsBodyHtml = true; //to make message body as html  
                mail.Body = GetHtmlBody(bookingDetails);
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(_appSettings.Value.Email, _appSettings.Value.Password);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(mail);
                return true;
            }
            catch (Exception)
            {

                throw;
            }

        }
        public static string GetHtmlBody(TrainBookingDetails bookingDetails)
        {
            try
            {
                string messageBody = "<font>Please find the ticket confirmation: </font><br><br>";
                if (bookingDetails == null) return messageBody;
                string htmlTableStart = "<table style=\"border-collapse:collapse; text-align:center;\" >";
                string htmlTableEnd = "</table>";
                string htmlHeaderRowStart = "<tr style=\"background-color:#6FA1D2; color:#ffffff;\">";
                string htmlHeaderRowEnd = "</tr>";
                string htmlTrStart = "<tr style=\"color:#555555;\">";
                string htmlTrEnd = "</tr>";
                string htmlTdStart = "<td style=\" border-color:#5c87b2; border-style:solid; border-width:thin; padding: 5px;\">";
                string htmlTdEnd = "</td>";
                messageBody += htmlTableStart;
                messageBody += htmlHeaderRowStart;
                messageBody += htmlTdStart + "Train Number" + htmlTdEnd;
                messageBody += htmlTdStart + "Train Name" + htmlTdEnd;
                messageBody += htmlTdStart + "Source" + htmlTdEnd;
                messageBody += htmlTdStart + "Destination" + htmlTdEnd;
                messageBody += htmlTdStart + "Date Of Journey" + htmlTdEnd;
                messageBody += htmlTdStart + "Booking Status" + htmlTdEnd;
                messageBody += htmlHeaderRowEnd;

                messageBody = messageBody + htmlTrStart;
                messageBody = messageBody + htmlTdStart + bookingDetails.TrainNumber + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + bookingDetails.TrainName + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + bookingDetails.Source + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + bookingDetails.Destination + htmlTdEnd;
                messageBody = messageBody + htmlTdStart + DateTime.Parse(bookingDetails.DateOfJourney.ToString())+ htmlTdEnd;
                messageBody = messageBody + htmlTdStart + "Success" + htmlTdEnd;
                messageBody = messageBody + htmlTrEnd;
                messageBody = messageBody + htmlTableEnd;
                return messageBody; 
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
