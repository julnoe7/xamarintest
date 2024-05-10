using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PassbookTicketGenerator.Models {

    [DataContract]
    public class EventTicket {

        [DataMember(Name = "headerFields", EmitDefaultValue = false)]
        public List<PassField> HeaderFields { get; set; }

        [DataMember(Name = "primaryFields", EmitDefaultValue = false)]
        public List<PassField> PrimaryFields { get; set; }

        [DataMember(Name = "secondaryFields", EmitDefaultValue = false)]
        public List<PassField> SecondaryFields { get; set; }

        [DataMember(Name = "auxiliaryFields", EmitDefaultValue = false)]
        public List<PassField> AuxiliaryFields { get; set; }

        [DataMember(Name = "backFields", EmitDefaultValue = false)]
        public List<PassField> BackFields { get; set; }
    }
}