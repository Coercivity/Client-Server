using System;
using System.Collections.Generic;

#nullable disable

namespace Client_Server_Theatre
{
    public partial class Administrator
    {
        public int Id { get; set; }

        public virtual Account IdNavigation { get; set; }
    }
}
