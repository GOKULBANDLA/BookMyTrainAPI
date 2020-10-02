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
         Task<DataTable> FetchTrains(int source, int destination);
         Task<List<BookingDetails>> FetchBookings(int source, int destination, DateTime datOfJourney);
        Task<List<Station>> FetchStations();
        Task<bool> BookTrain(TrainBookingDetails bookingDetails);

    }
}
