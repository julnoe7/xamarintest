using System;
using ElectronicTicketGenerator;

namespace xamarin.Models
{
    public class TransferTicketData
    {

        public long SaleId { get; set; }
        public long ClientOwnerId { get; set; }
        public string SectionDescription { get; set; }
        public string BlockName { get; set; }
        public string TicketDescription { get; set; }
        public long? AssignedClientId { get; set; }
        public string EventDateTimeDescription { get; set; }
        public string EventName { get; set; }
        public string MailImage { get; set; }
    }

    public class TicketData : ITicket
    {

        public int EventId { get; set; }
        public long SaleId { get; set; }
        public long SaleDescriptionId { get; set; }
        public string Barcode { get; set; }
        public string SectionLabel { get; set; }
        public string BlockLabel { get; set; }
        public string TypeLabel { get; set; }
        public string RowLabel { get; set; }
        public string SeatLabel { get; set; }
        public string EventName { get; set; }
        public DateTime EventDateTime { get; set; }
        public string EventDateTimeDescription { get; set; }
        public string EventTime { get; set; }
        public string Venue { get; set; }
        public string VenueAddress { get; set; }
        public string City { get; set; }
        public string EventImage { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string ClientName { get; set; }
        public string PaymentType { get; set; }
        public decimal Price { get; set; }
        public decimal Charge { get; set; }
        public string PriceLegend { get; set; }
        public string AccessLabel { get; set; }
        public string TaxLegend { get; set; }
        public string ControlCode { get; set; }
        public string PrintDateTime { get; set; }
        public string ConsecutiveNumber { get; set; }
        public string BusinessName { get; set; }
        public int MinutesToStart { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyKey { get; set; }
        public string CurrencySymbol { get; set; }
        public string CurrencyMask { get; set; }
        public string LocationCulture { get; set; }
        public bool IsDownloadAvailable { get; set; }
        public bool IsPrintTicketAvailable { get; set; }
    }
}

