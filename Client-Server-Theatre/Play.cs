using System;
using System.Collections.Generic;

#nullable disable

namespace Client_Server_Theatre
{
    public partial class Play
    {
        public Play()
        {
            Tickets = new HashSet<Ticket>();
        }

        public string PlayName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int? TicketAmount { get; set; }
        public int Id { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}
