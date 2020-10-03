using ModelLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public interface IBusinessManager
    {
        Task<List<TrainDetails>> FetchTrains(TrainSearch search);
        Task<List<Station>> FetchStations();
        Task<bool> BookTrain(TrainBookingDetails bookingDetails);
        bool SendEmail(TrainBookingDetails bookingDetails);
    }
}
