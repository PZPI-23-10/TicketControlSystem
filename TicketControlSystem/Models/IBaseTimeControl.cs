namespace TicketControlSystem.Data.Models;

public interface IBaseTimeControl
{
     DateTimeOffset Created { get; set; }
     DateTimeOffset LastModified { get; set; }
}