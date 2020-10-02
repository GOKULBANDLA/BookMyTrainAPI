using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer
{
   public class BookingDetails
    {
        public int BookingId { get; set; }
        public int TrainId { get; set; }
        public int Source { get; set; }
        public int Destination { get; set; }
        public string DateOfJourney { get; set; }

    }
}
