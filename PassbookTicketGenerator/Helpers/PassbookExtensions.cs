using ElectronicTicketGenerator;
using PassbookTicketGenerator.Enums;
using PassbookTicketGenerator.Models;
using System.Collections.Generic;

namespace PassbookTicketGenerator.Helpers {

    public static class PassbookExtensions {

        #region consts
        private const string PASS_DATEFORMAT = "yyyy-MM-ddTHH:mm-00:00";
        private const string PASS_FORECOLOR = "rgb(255, 255, 255)";
        private const string PASS_BACKCOLOR = "rgb(0, 70, 127)";
        private const string PASS_DESCRIPTION = "Boleto Pocket-Ticket";
        private const string PASS_CONTENTKEY_HEADERTIME = "headerDateTime";
        private const string TEXT_ALIGNMENT_RIGHT = "PKTextAlignmentRight";
        private const string PASS_CONTENT_KEY_EVENT = "event";
        private const string PASS_CONTENT_KEY_SEAT = "asiento";
        private const string PASS_CONTENT_KEY_SECTION = "seccion";
        private const string PASS_CONTENT_KEY_ENTRANCE = "acceso";
        private const string PASS_CONTENT_KEY_DATETIME = "dateTime";
        private const string PASS_DATESTYLE_SHORT = "PKDateStyleShort";
        private const string PASS_CONTENT_KEY_TYPE = "tipo";
        //private const string PASS_CONTENT_KEY_INSTRUCTIONS = "instrucciones";
        //private const string PASS_CONTENT_KEY_POLICIES = "politicas";
        private const string PASS_CONTENT_KEY_NOTES = "nota";
        private const string PASS_CONTENT_KEY_TERMS = "terms";
        #endregion

        #region helpers
        public static Models.Passbook ToPassbook(this ITicket ticket, in IDictionary<string, string> passbookConfig) {

            var pass = new Models.Passbook {
                FileName = $"{ticket.EventName}-{ticket.SeatLabel}",
                Extension = "pkpass",
                FormatVersion = 2,
                Identifier = passbookConfig["PassIdentifier"],
                OrganizationName = passbookConfig["OrganizationName"],
                TeamIdentifier = passbookConfig["TeamIdentifier"],
                Description = PASS_DESCRIPTION,
                BackgroundColor = PASS_BACKCOLOR,
                ForegroundColor = PASS_FORECOLOR,
                LabelColor = PASS_FORECOLOR,
                Barcode = new Barcode(BarcodeType.PKBarcodeFormatQR, ticket.Barcode, "iso-8859-1", ticket.Barcode),
                //TODO fix this
                RelevantDate = ticket.EventDateTime.ToUniversalTime().ToString(PASS_DATEFORMAT),
                SerialNumber = ticket.SaleDescriptionId.ToString(),
                IsSharingProhibited = true
            };

            pass.Barcodes.Add(
                new Barcode(
                    BarcodeType.PKBarcodeFormatQR,
                    ticket.Barcode,
                    "iso-8859-1",
                    ticket.Barcode));

            //if (ticket.EventId == 1524) {
            //    pass.Nfc.Add(
            //        new Nfc {
            //            Message = $"{ticket.Barcode}{ticket.Barcode}{ticket.Barcode}{ticket.Barcode}"
            //        });
            //    pass.Barcode = null;
            //    pass.Barcodes.Clear();
            //}

            if (ticket.Latitude is not null && ticket.Longitude is not null) {
                pass.Locations.Add(new Location((decimal)ticket.Latitude, (decimal)ticket.Longitude));
            }

            pass.EventTicket.HeaderFields.Add(
                new PassField(
                    PASS_CONTENTKEY_HEADERTIME,
                    ticket.EventDateTime.ToString("dd MMM yyyy"),
                    ticket.EventDateTime.ToString("HH:mm") + " HRS") {
                    TextAlign = TEXT_ALIGNMENT_RIGHT
                });

            pass.EventTicket.SecondaryFields.AddRange(
                new[] {
                    new PassField(
                        PASS_CONTENT_KEY_EVENT,
                        ticket.Venue.Length <= 30 ? ticket.Venue : ticket.Venue.Substring(0,30),
                        ticket.EventName.Length <= 30 ? ticket.EventName : ticket.EventName.Substring(0,30)) {
                        Semantics = new SemanticsValue {
                            EventStartDate = ticket.EventDateTime.ToUniversalTime().ToString(PASS_DATEFORMAT)
                        },
                    },
                    new PassField(
                        PASS_CONTENT_KEY_SEAT,
                        passbookConfig["SeatLabel"],
                        $"{ticket.RowLabel}-{ticket.SeatLabel}") { TextAlign = TEXT_ALIGNMENT_RIGHT }
                });

            pass.EventTicket.AuxiliaryFields.AddRange(
                new[] {
                    new PassField(
                        PASS_CONTENT_KEY_SECTION,
                        passbookConfig["SectionBlockLabel"],
                        ticket.SectionLabel + " / " + ticket.BlockLabel),
                    new PassField(
                        PASS_CONTENT_KEY_ENTRANCE,
                        passbookConfig["EntranceLabel"],
                        ticket.AccessLabel) { TextAlign = TEXT_ALIGNMENT_RIGHT }
                });

            pass.EventTicket.BackFields.AddRange(
                new[] {
                    new PassField(
                        PASS_CONTENT_KEY_DATETIME,
                        passbookConfig["ReminderLabel"],
                        ticket.EventDateTime.ToUniversalTime().ToString(PASS_DATEFORMAT)) {
                            DateStyle = PASS_DATESTYLE_SHORT,
                            TimeStyle = PASS_DATESTYLE_SHORT
                    },
                    new PassField(
                        PASS_CONTENT_KEY_TYPE,
                        passbookConfig["TypeLabel"],
                        ticket.TypeLabel),
                    new PassField(
                        "ticketHolder",
                        "Cliente",
                        ticket.ClientName),
                    new PassField(
                        "ticketNumber",
                        "Folio",
                        $"{ticket.SaleId}-{ticket.SaleDescriptionId}"),
                    new PassField(
                        "venue",
                        "Inmueble",
                        ticket.Venue),
                    new PassField(
                        "venueAddress",
                        "Dirección",
                        ticket.VenueAddress),
                    new PassField(
                        PASS_CONTENT_KEY_NOTES,
                        passbookConfig["NotesLabel"],
                        passbookConfig["NotesDescription"]),
                    new PassField(
                        PASS_CONTENT_KEY_TERMS,
                        passbookConfig["TermsLabel"],
                        passbookConfig["TermsDescription"])
                    //new PassField(
                    //    PASS_CONTENT_KEY_INSTRUCTIONS,
                    //    passbookConfig["InstructionsLabel"],
                    //    passbookConfig["instructionsDescription"]),
                    //new PassField(
                    //    PASS_CONTENT_KEY_POLICIES,
                    //    passbookConfig["PoliciesLabel"],
                    //    passbookConfig["PoliciesDescription"]),
                });

            return pass;
        }
        #endregion
    }
}