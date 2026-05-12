using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TicketControlSystem.Data.Models;

public class User : IdentityUser<int>, IBaseTimeControl
{
        public ICollection<Event> OwnedEvents { get; set; } = new List<Event>();
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastModified { get; set; }
}
