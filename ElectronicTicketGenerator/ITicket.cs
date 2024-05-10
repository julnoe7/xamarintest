using System;

namespace ElectronicTicketGenerator {

    public interface ITicket {

        #region refactored properties
        int EventId { get; set; }
        long SaleId { get; set; }
        long SaleDescriptionId { get; set; }
        string Barcode { get; set; }
        string SectionLabel { get; set; }
        string BlockLabel { get; set; }
        string RowLabel { get; set; }
        string TypeLabel { get; set; }
        string SeatLabel { get; set; }
        string EventName { get; set; }
        DateTime EventDateTime { get; set; }
        string EventDateTimeDescription { get; set; }
        string EventTime { get; set; }
        string Venue { get; set; }
        string VenueAddress { get; set; }
        string City { get; set; }
        string EventImage { get; set; }
        double? Latitude { get; set; }
        double? Longitude { get; set; }
        string ClientName { get; set; }
        string PaymentType { get; set; }
        decimal Price { get; set; }
        decimal Charge { get; set; }
        string PriceLegend { get; set; }
        string AccessLabel { get; set; }
        string TaxLegend { get; set; }
        string ControlCode { get; set; }
        string PrintDateTime { get; set; }
        string ConsecutiveNumber { get; set; }
        string BusinessName { get; set; }
        int MinutesToStart { get; set; }
        int CurrencyId { get; set; }
        string CurrencyKey { get; set; }
        string CurrencySymbol { get; set; }
        string CurrencyMask { get; set; }
        string LocationCulture { get; set; }
        #endregion
    }
}