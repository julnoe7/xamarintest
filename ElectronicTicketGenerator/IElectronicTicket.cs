namespace ElectronicTicketGenerator {

     public interface IElectronicTicket {
         byte[] FileContent { get; set; }
         string FileName { get; set; }
         string Extension { get; set; }
    }
}