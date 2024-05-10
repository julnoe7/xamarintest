using System.Runtime.Serialization;

namespace PassbookTicketGenerator.Models {

    [DataContract]
    public class Location {

        #region properties
        [DataMember(Name = "latitude")]
        public decimal Latitude { get; set; }
        [DataMember(Name = "longitude")]
        public decimal Longitude { get; set; }
        #endregion

        #region constructor
        public Location(decimal latitude, decimal longitude) {
            Latitude = latitude;
            Longitude = longitude;
        }
        #endregion
    }
}
