using ElectronicTicketGenerator;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PassbookTicketGenerator.Models {

    [DataContract(Name = "Pass")]
    public class Passbook : IElectronicTicket {

        [IgnoreDataMember]
        public byte[] FileContent { get; set; }
        [IgnoreDataMember]
        public string FileName { get; set; }
        [IgnoreDataMember]
        public string Extension { get; set; }

        #region Standard Keys
        [DataMember(Name = "passTypeIdentifier")]
        public string Identifier { get; set; }
        [DataMember(Name = "formatVersion")]
        public int FormatVersion { get; set; }
        [DataMember(Name = "serialNumber")]
        public string SerialNumber { get; set; }
        [DataMember(Name = "description")]
        public string Description { get; set; }
        [DataMember(Name = "teamIdentifier")]
        public string TeamIdentifier { get; set; }
        [DataMember(Name = "organizationName")]
        public string OrganizationName { get; set; }
        [DataMember(Name = "sharingProhibited")]
        public bool IsSharingProhibited { get; set; }
        #endregion

        #region Relevance Keys
        [DataMember(Name = "relevantDate")]
        public string RelevantDate { get; set; }
        //[DataMember(Name = "expirationDate")]
        //public string ExpirationDate { get; set; }
        //[DataMember(Name = "voided")]
        //public bool IsVoided { get; set; }
        [DataMember(Name = "locations")]
        public List<Location> Locations { get; set; } = new();

        #endregion

        #region Visual Appearance Keys
        [DataMember(Name = "backgroundColor")]
        public string BackgroundColor { get; set; }
        [DataMember(Name = "foregroundColor")]
        public string ForegroundColor { get; set; }
        [DataMember(Name = "labelColor")]
        public string LabelColor { get; set; }
        [DataMember(Name = "barcode")]
        public Barcode Barcode { get; set; }
        [DataMember(Name = "barcodes")]
        public List<Barcode> Barcodes { get; set; } = new();

        #endregion

        #region NFC-Enabled Pass Keys
        //[DataMember(Name = "nfc")] 
        //public List<Nfc> Nfc { get; set; } = new();
        #endregion

        #region Style Keys
        [DataMember(Name = "eventTicket")]
        public EventTicket EventTicket { get; set; } = new() {
            HeaderFields = new List<PassField>(),
            PrimaryFields = new List<PassField>(),
            SecondaryFields = new List<PassField>(),
            AuxiliaryFields = new List<PassField>(),
            BackFields = new List<PassField>()
        };

        #endregion

        //#region Constructor
        //public Passbook() {
        //    Barcodes = new List<Barcode>();

        //    EventTicket = new EventTicket {
        //        HeaderFields = new List<PassField>(),
        //        PrimaryFields = new List<PassField>(),
        //        SecondaryFields = new List<PassField>(),
        //        AuxiliaryFields = new List<PassField>(),
        //        BackFields = new List<PassField>()
        //    };

        //    Locations = new List<Location>();
        //}
        //#endregion
    }
}