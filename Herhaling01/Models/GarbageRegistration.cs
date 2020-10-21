using System;
using System.Collections.Generic;
using System.Text;

namespace Herhaling01.Models
{
    class GarbageRegistration
    {
        public Guid GarbageRegistrationId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public Guid GarbageTypeId { get; set; }
        public Guid CityId { get; set; }
        public string Street { get; set; }
        public double Weight { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
