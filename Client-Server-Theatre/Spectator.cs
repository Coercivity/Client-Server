using System;
using System.Collections.Generic;

#nullable disable

namespace Client_Server_Theatre
{
    public partial class Spectator
    {
        public Spectator()
        {
            Tickets = new HashSet<Ticket>();
        }

        public string FullName { get; set; }
        public int Id { get; set; }

        public virtual Account IdNavigation { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}
