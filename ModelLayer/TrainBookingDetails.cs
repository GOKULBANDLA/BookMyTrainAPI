using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer
{
   public class TrainBookingDetails
    {
        public string TrainNumber { get; set; }
        public string TrainName { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string DateOfJourney { get; set; }
        public int TrainId { get; set; }
        public int SourceId { get; set; }
        public int DestinationId { get; set; }
    }
}
