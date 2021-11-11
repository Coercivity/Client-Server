using System;
using System.Collections.Generic;

#nullable disable

namespace Server
{
    public partial class Ticket
    {
        public decimal? Price { get; set; }
        public int? Place { get; set; }
        public int Id { get; set; }
        public int? Client { get; set; }
        public int? Play { get; set; }
        public virtual Spectator ClientNavigation { get; set; }
        public virtual Play PlayNavigation { get; set; }
    }
}
