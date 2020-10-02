using System;

namespace ModelLayer
{
    public class TrainDetails
    {
        public string TrainNumber { get; set; }
        public string TrainName { get; set; }
        public int Seats { get; set; }
        public int TrainId { get; set; }

        public int RouteId { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string Departure { get; set; }
        public string Arrival { get; set; }
        public bool IsAvailable { get; set; }

    }
}
