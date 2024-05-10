using PassbookTicketGenerator.Enums;
using System;
using System.Runtime.Serialization;

namespace PassbookTicketGenerator.Models {

    [DataContract]
    public class Barcode {

        public BarcodeType BarcodeType { get; set; }

        [DataMember(Name = "format")]
        public string Format {
            get => Enum.GetName(typeof(BarcodeType), BarcodeType);
            set => BarcodeType = (BarcodeType)Enum.Parse(typeof(BarcodeType), value);
        }

        [DataMember(Name = "message")]
        public string Message { get; set; }
        [DataMember(Name = "messageEncoding")]
        public string Encoding { get; set; }
        [DataMember(Name = "altText")]
        public string AltText { get; set; }

        public Barcode(BarcodeType type, string message, string encoding, string altText) {
            BarcodeType = type;
            Message = message;
            Encoding = encoding;
            AltText = altText;
        }
    }

    [DataContract]
    public class Nfc {
        [DataMember(Name = "message")]
        public string Message { get; set; }
    }
}
