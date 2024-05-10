using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectronicTicketGenerator {

    public interface IElectronicTicketGenerator {

        Task<IElectronicTicket> GenerateAsync(ITicket ticket);

        Task<IElectronicTicket> GenerateAsync(IReadOnlyCollection<ITicket> tickets);

        Task<IEnumerable<IElectronicTicket>> GenerateTicketsAsync(IReadOnlyCollection<ITicket> tickets);
    }
}