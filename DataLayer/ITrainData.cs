using ModelLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public interface ITrainData
    {
         Task<DataTable> FetchTrains(TrainSearch search);
         Task<List<BookingDetails>> FetchBookings(TrainSearch search);
        Task<List<Station>> FetchStations();
        Task<bool> BookTrain(TrainBookingDetails bookingDetails);

    }
}
